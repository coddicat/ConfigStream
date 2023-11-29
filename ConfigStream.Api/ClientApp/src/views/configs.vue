<script setup lang="ts">
import { useConfigStore } from '@/store/config';
import { type ConfigValue, useConfigValueStore } from '@/store/config-value';
import { storeToRefs } from 'pinia';
import type { TreeNode } from 'primevue/tree';
import { onMounted, computed, nextTick, reactive, ref } from 'vue';
import { groupBy } from 'lodash';
import ConfigValueComponent from '@/components/config-value.vue';
import ConfigValueChip from '@/components/config-value-chip.vue';
import ConfigActions from '@/components/config-actions.vue';
import ConfigsToolbar from '@/components/configs-toolbar.vue';
import { sort } from '@/utils/array';

const configStore = useConfigStore();
const configValueStore = useConfigValueStore();
const {
  selectedEnvironments,
  items: configValues,
  loading
  // search
} = storeToRefs(configValueStore);
const { items: configs } = storeToRefs(configStore);
const { requestConfigValueList } = configValueStore;
const { requestConfigList } = configStore;

onMounted(async () => {
  await requestConfigList();
  await requestConfigValueList();
  nextTick(() => {
    selectedEnvironments.value =
      environments.value.length > 0 ? [environments.value[0]] : [];
  });
});

const nodes = computed<TreeNode[]>(() => {
  const noValue = configs.value
    .filter(
      x =>
        !configValues.value.some(
          y => x.groupName === y.groupName && x.name === y.configName
        )
    )
    .map(x =>
      reactive({
        groupName: x.groupName,
        configName: x.name,
        environmentName: 'undefined'
      })
    );

  const list = sort(
    [...configValues.value, ...noValue],
    x => `${x.groupName}:${x.configName}`
  );
  return groupByGroupName(list);
});
const editNodes = ref<TreeNode[]>([]);
const expandedKeys = ref<Record<string, boolean>>({});

const environments = computed(() => {
  return Array.from(
    new Set(configValues.value.map(item => item.environmentName))
  );
});

function getConfig(groupName: string, configName: string) {
  return (
    configs.value.find(
      y => y.groupName === groupName && y.name === configName
    ) ?? reactive({ deleted: true })
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
      reactive<ConfigValue>({
        configName,
        groupName,
        environmentName: env,
        targetName,
        value: undefined
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
  return targetNames.map(x =>
    reactive({
      label: x,
      key: `${groupName}:${configName}:${x}`,
      type: 'target',
      data: reactive({
        config: getConfig(groupName, configName),
        configValues: getEnvironmentValues(groupName, configName, x, grouped[x])
      })
    })
  );
}

function groupByConfigName(
  groupName: string,
  values: ConfigValue[]
): TreeNode[] {
  const grouped = groupBy(values, x => x.configName);
  return Object.getOwnPropertyNames(grouped).map(x =>
    reactive({
      key: `${groupName}:${x}`,
      label: x,
      type: 'config',
      data: reactive({
        config: getConfig(groupName, x),
        configValues: getEnvironmentValues(
          groupName,
          x,
          undefined,
          grouped[x].filter(y => !y.targetName)
        )
      }),
      children: groupByTargetName(groupName, x, grouped[x])
    })
  );
}

function groupByGroupName(values: ConfigValue[]): TreeNode[] {
  const grouped = groupBy(values, x => x.groupName);
  return Object.getOwnPropertyNames(grouped).map(x =>
    reactive({
      key: x,
      label: x,
      type: 'group',
      children: groupByConfigName(x, grouped[x])
    })
  );
}

function expand(groupName: string, configName?: string) {
  expandedKeys.value[groupName] = true;
  if (!configName) return;
  expandedKeys.value[`${groupName}:${configName}`] = true;
}
</script>

<template>
  <main>
    <TreeTable
      :value="nodes"
      :loading="loading"
      v-model:expanded-keys="expandedKeys"
      loadingIcon="pi pi-spin pi-spinner"
      class="p-treetable-sm"
      resizable-columns
      show-gridlines
    >
      <template #header>
        <ConfigsToolbar
          :environments="environments"
          v-model:selected-environments="selectedEnvironments"
          :config-values="configValues"
          :configs="configs"
          @expand="expand"
        />
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
          <div
            v-if="node.type === 'config'"
            class="w-full text-overflow-ellipsis white-space-nowrap overflow-hidden"
          >
            {{ node.data.config.description }}
          </div>
        </template>
      </Column>
      <Column header="Default Value" body-class="text-center">
        <template #body="{ node }">
          <ConfigValueChip
            v-if="node.type === 'config'"
            :label="node.data.config.defaultValue"
          />
        </template>
      </Column>
      <Column
        v-for="env in selectedEnvironments"
        :key="env"
        :header="env"
        class="bg-primary-reverse"
        body-class="text-center"
      >
        <template #body="{ node }">
          <ConfigValueComponent
            v-if="['config', 'target'].includes(node.type)"
            :config="node.data.config"
            :config-value="node.data.configValues[env]"
            :edit="editNodes.includes(node)"
            :disabled="node.data?.config.deleted"
          />
        </template>
      </Column>
      <Column headerStyle="width: 10rem">
        <template #body="{ node }">
          <ConfigActions
            :node="node"
            :config-values="configValues"
            :environments="environments"
            :edit-nodes="editNodes"
            @expand="expand"
          />
        </template>
      </Column>
    </TreeTable>
  </main>
</template>
