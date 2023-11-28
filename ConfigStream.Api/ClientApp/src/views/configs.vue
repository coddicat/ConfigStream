<script setup lang="ts">
import { Config, useConfigStore } from '@/store/config';
import { ConfigValue, useConfigValueStore } from '@/store/config-value';
import { storeToRefs } from 'pinia';
import type { TreeNode } from 'primevue/tree';
import { onMounted, computed, nextTick, reactive, ref } from 'vue';
import { groupBy } from 'lodash';
import ConfigValueComponent from '@/components/config-value.vue';
import { confirmDialog, promptDialog } from '@/utils/dialog';
import { MenuItem } from 'primevue/menuitem';
import EllipsisMenu from '@/components/ellipsis-menu.vue';
import SetupConfigDialog from '@/components/setup-config-dialog.vue';
import { isValidRedisKey } from '@/utils/redisKey';
import ConfigValueChip from '@/components/config-value-chip.vue';

const configStore = useConfigStore();
const configValueStore = useConfigValueStore();
const {
  selectedEnvironments,
  items: configValues,
  loading
  // search
} = storeToRefs(configValueStore);
const { items: configs } = storeToRefs(configStore);
const { requestConfigValueList, submitConfigValues } = configValueStore;
const { requestConfigList, deleteConfig, createOrUpdateConfig } = configStore;

function sort<T>(arr: T[], predicate: (v: T) => string): T[] {
  return arr.map(x => x).sort((x, y) => (predicate(x) > predicate(y) ? 1 : -1));
}

// const configs = computed(() =>
//   sort(_configs.value, x => `${x.groupName}:${x.name}`)
// );

// const configValues = computed(() =>
//   sort(
//     _configValues.value,
//     x => `${x.groupName}:${x.configName}:${x.targetName}`
//   )
// );

onMounted(async () => {
  await requestConfigList();
  await requestConfigValueList();
  nextTick(() => {
    selectedEnvironments.value =
      environments.value.length > 0 ? [environments.value[0]] : [];
  });
});

const environments = computed(() => {
  return Array.from(
    new Set(configValues.value.map(item => item.environmentName))
  );
});

function getConfig(groupName: string, configName: string) {
  return (
    configs.value.find(
      y => y.groupName === groupName && y.name === configName
    ) ?? { deleted: true }
  );
}

function getEnvironmentValues(
  groupName: string,
  configName: string,
  targetName: string | undefined,
  values: ConfigValue[]
) {
  const vals: Record<string, ConfigValue> = {};
  selectedEnvironments.value.forEach(env => {
    vals[env] =
      values.find(x => x.environmentName === env) ??
      reactive({
        configName,
        groupName,
        environmentName: env,
        targetName,
        value: null
      });
  });
  return vals;
}

function groupByTargetName(
  groupName: string,
  configName: string,
  values: ConfigValue[]
): TreeNode[] {
  const grouped = groupBy(
    values.filter(x => !!x.targetName),
    x => x.targetName
  );
  const targetNames = sort(Object.getOwnPropertyNames(grouped), x => x);
  return targetNames.map(x => ({
    label: x,
    key: `${groupName}:${configName}:${x}`,
    type: 'target',
    data: {
      config: getConfig(groupName, configName),
      configValues: getEnvironmentValues(groupName, configName, x, grouped[x])
    }
  }));
}

function groupByConfigName(
  groupName: string,
  values: ConfigValue[]
): TreeNode[] {
  const grouped = groupBy(values, x => x.configName);
  return Object.getOwnPropertyNames(grouped).map(x => ({
    key: `${groupName}:${x}`,
    label: x,
    type: 'config',
    data: {
      config: getConfig(groupName, x),
      configValues: getEnvironmentValues(
        groupName,
        x,
        undefined,
        grouped[x].filter(y => !y.targetName)
      )
    },
    children: groupByTargetName(groupName, x, grouped[x])
  }));
}

function groupByGroupName(values: ConfigValue[]): TreeNode[] {
  const grouped = groupBy(values, x => x.groupName);
  return Object.getOwnPropertyNames(grouped).map(x => ({
    key: x,
    label: x,
    type: 'group',
    children: groupByConfigName(x, grouped[x])
  }));
}

const nodes = computed<TreeNode[]>(() => {
  const noValue = configs.value
    .filter(
      x =>
        !configValues.value.some(
          y => x.groupName === y.groupName && x.name === y.configName
        )
    )
    .map(x => ({
      groupName: x.groupName,
      configName: x.name,
      environmentName: 'undefined'
    }));

  const list = sort(
    [...configValues.value, ...noValue],
    x => `${x.groupName}:${x.configName}`
  );
  return groupByGroupName(list);
});

const editNodes = ref<TreeNode[]>([]);
function onEditNode(node: TreeNode) {
  editNodes.value.push(node);
  const json = JSON.stringify(node.data.configValues);
  node.data.originConfigValues = json;
}

function onCancelEditNode(node: TreeNode) {
  const index = editNodes.value.indexOf(node);
  editNodes.value.splice(index, 1);
  const origin = reactive(JSON.parse(node.data.originConfigValues));
  node.data.configValues = origin;
}

async function onSaveNode(node: TreeNode) {
  const index = editNodes.value.indexOf(node);
  editNodes.value.splice(index, 1);
  delete node.data.originConfigValues;
  const configValues = Object.getOwnPropertyNames(node.data.configValues).map(
    x => node.data.configValues[x] as ConfigValue
  );
  await submitConfigValues(configValues);
}

async function onDelete(node: TreeNode) {
  const yes = await confirmDialog(
    `Do you want to delete '${node.data.config.name}' config? (values will be preserved)`,
    'Delete Confirmation'
  );
  if (!yes) return;

  await deleteConfig(node.data.config);
}

async function onAddTarget(node: TreeNode): Promise<void> {
  const res = await promptDialog(
    'Enter a new target name',
    'Adding a new target',
    (value?: string) => {
      if (!value) return 'Field is required';
      return isValidRedisKey(value) || 'Invalid format';
    }
  );
  if (!res) return;

  environments.value.forEach(env =>
    configValues.value.push({
      configName: node.data.config.name,
      groupName: node.data.config.groupName,
      environmentName: env,
      targetName: res
    })
  );

  nextTick(() => {
    expandGroup(node.data.config.groupName);
    expandConfig(node.data.config.groupName, node.data.config.name);
  });
}

function getMenuItems(node: TreeNode): MenuItem[] {
  return [
    {
      label: 'New target',
      icon: 'pi pi-stop-circle',
      command: () => onAddTarget(node)
    },
    {
      separator: true
    },
    {
      label: 'Setup config',
      icon: 'pi pi-cog'
    },
    {
      label: 'Delete config',
      icon: 'pi pi-trash',
      command: () => onDelete(node)
    }
  ];
}

async function addEnvironment() {
  const res = await promptDialog(
    'Enter a new environment name',
    'Adding a new environment'
  );
  if (!res) return;
  configValues.value.push({
    configName: configs.value[0].name,
    groupName: configs.value[0].groupName,
    environmentName: res
  });
  selectedEnvironments.value.push(res);
}

const setupConfig = ref(false);
function addConfig() {
  setupConfig.value = true;
}
async function onSubmitSetupConfig(config: Config) {
  await createOrUpdateConfig(config);
  expandGroup(config.groupName);
}

const expandedKeys = ref<Record<string, boolean>>({});
function expandGroup(groupName: string) {
  if (expandedKeys.value[groupName]) return;
  expandedKeys.value[groupName] = true;
}
function expandConfig(groupName: string, configName: string) {
  if (expandedKeys.value[`${groupName}:${configName}`]) return;
  expandedKeys.value[`${groupName}:${configName}`] = true;
}
</script>

<template>
  <main>
    <SetupConfigDialog v-model="setupConfig" @submit="onSubmitSetupConfig" />
    <TreeTable
      :value="nodes"
      :loading="loading"
      v-model:expanded-keys="expandedKeys"
      loadingIcon="pi pi-spin pi-spinner"
      class="p-treetable-sm"
      resizable-columns
      show-gridlines
      paginator
      :rows="5"
      :rowsPerPageOptions="[5, 10, 25]"
    >
      <template #header>
        <Toolbar class="border-none p-0">
          <!-- <template #center>
            <div class="p-input-icon-left">
              <i class="pi pi-search"></i>
              <InputText placeholder="Config name" :model-value="search" />
            </div>
          </template> -->
          <template #start>
            <Button
              @click="addConfig"
              label="New config"
              icon="pi pi-plus"
              size="small"
            ></Button>
          </template>
          <template #end>
            <Button
              @click="addEnvironment"
              label="New environment"
              size="small"
              icon="pi pi-plus"
              :disabled="configs.length === 0"
              class="mr-3"
            ></Button>
            <MultiSelect
              v-model="selectedEnvironments"
              :options="environments"
              class="w-full sm:w-26rem"
              display="chip"
            >
            </MultiSelect>
          </template>
        </Toolbar>
      </template>

      <Column header="Name" expander>
        <template #body="{ node }">
          <i
            v-if="node.type === 'target'"
            class="pi pi-stop-circle mr-2"
            style="font-size: 0.8rem"
          ></i>
          <span :class="{ 'line-through': node.data?.config.deleted }">
            {{ node.label }}
          </span>
        </template>
      </Column>
      <Column header="Description">
        <template #body="{ node }">
          <span v-if="node.type === 'config'">
            {{ node.data.config.description }}
          </span>
        </template>
      </Column>
      <Column header="Default Value">
        <template #body="{ node }">
          <ConfigValueChip
            v-if="node.type === 'config'"
            :label="node.data.config.defaultValue"
          />
        </template>
      </Column>
      <Column v-for="env in selectedEnvironments" :key="env" :header="env">
        <template #body="{ node }">
          <template v-if="['config', 'target'].includes(node.type)">
            <ConfigValueComponent
              :config="node.data.config"
              :config-value="node.data.configValues[env]"
              :edit="editNodes.includes(node)"
              :disabled="node.data?.config.deleted"
            ></ConfigValueComponent>
          </template>
        </template>
      </Column>
      <Column headerStyle="width: 10rem">
        <template #body="{ node }">
          <div class="flex flex-row-reverse">
            <!-- <template v-if="node.type === 'config'">
              <EllipsisMenu
                :items="getMenuItems(node)"
                :disabled="editNodes.includes(node)"
              />
            </template> -->
            <!-- <template v-else-if="node.type === 'target'">
              <Button
                :disabled="editNodes.includes(node)"
                icon="pi pi-trash"
                text
                size="small"
                rounded
                severity="danger"
                title="Delete target"
                class="flex-shrink-0"
              />
            </template> -->
            <template v-if="['config', 'target'].includes(node.type)">
              <EllipsisMenu
                :items="getMenuItems(node)"
                :disabled="editNodes.includes(node) || node.type === 'target'"
              />
              <Button
                v-if="!editNodes.includes(node)"
                @click="onEditNode(node)"
                icon="pi pi-pencil"
                text
                size="small"
                rounded
                severity="info"
                title="Edit values"
                class="flex-shrink-0"
              />
              <template v-else>
                <Button
                  @click="onCancelEditNode(node)"
                  icon="pi pi-times"
                  text
                  size="small"
                  rounded
                  severity="warning"
                  class="flex-shrink-0"
                  title="Cancel editing"
                />
                <Button
                  @click="onSaveNode(node)"
                  icon="pi pi-save"
                  text
                  size="small"
                  rounded
                  severity="success"
                  class="flex-shrink-0"
                  title="Save values"
                />
              </template>
            </template>
          </div>
        </template>
      </Column>
    </TreeTable>
  </main>
</template>
