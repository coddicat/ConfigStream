<script setup lang="ts">
import { type ConfigValue, useConfigValueStore } from '@/store/config-value';
import { type Config, useConfigStore } from '@/store/config';
import { confirmDialog, promptDialog } from '@/utils/dialog';
import { isValidRedisKey } from '@/utils/redisKey';
import { type MenuItem } from 'primevue/menuitem';
import { TreeNode } from 'primevue/tree';
import { computed, reactive } from 'vue';
import EllipsisMenu from './ellipsis-menu.vue';
import { storeToRefs } from 'pinia';
import { useHomeStore } from '@/store/home';

const configValueStore = useConfigValueStore();
const homeStore = useHomeStore();

const { sortedEnvironments, selectedEnvironments } = storeToRefs(homeStore);
const { submitConfigValues, addConfigValue } = configValueStore;
const { deleteConfig } = useConfigStore();
const {
  editConfigValues,
  cancelEditConfigValues,
  expand,
  isEditing,
  openSetupConfigDialog
} = homeStore;

const props = defineProps<{
  node: TreeNode;
}>();

const data = computed<{
  config: Config;
  configValues: ConfigValue[];
}>(() => props.node.data);
const nodeType = computed(() => props.node.type);
const target = computed(() =>
  nodeType.value === 'target' ? props.node.label : undefined
);
const editConfigData = computed(() => ({
  configName: data.value.config.configName,
  groupName: data.value.config.groupName,
  targetName: target.value
}));
const isEdit = computed(() => isEditing(editConfigData.value));
const values = computed(() =>
  Object.getOwnPropertyNames(data.value.configValues).map(
    x => data.value.configValues[x] as ConfigValue
  )
);

const temporary = computed(
  () =>
    (nodeType.value === 'target' || data.value.config.deleted) &&
    values.value.filter(x => x.value !== undefined && x.value !== null)
      .length === 0
);

const menuItems = computed<MenuItem[]>(() => [
  {
    label: 'New target',
    icon: 'pi pi-stop-circle',
    command: () => onAddTarget()
  },
  {
    separator: true
  },
  {
    label: 'Setup config',
    icon: 'pi pi-cog',
    command: () => openSetupConfigDialog(data.value.config),
    disabled: data.value.config.deleted
  },
  {
    label: 'Delete config',
    icon: 'pi pi-trash',
    command: () => onDelete(),
    disabled: data.value.config.deleted
  }
]);

function onEditNode() {
  const json = JSON.stringify(data.value.configValues);
  editConfigValues(editConfigData.value, json);
}

function onCancelEditNode() {
  const json = cancelEditConfigValues(editConfigData.value);
  if (!json) return;

  const origin = reactive(JSON.parse(json));
  data.value.configValues = origin;
}

async function onSaveNode() {
  const configValues = Object.getOwnPropertyNames(data.value.configValues).map(
    x => data.value.configValues[x] as ConfigValue
  );

  await submitConfigValues(configValues);

  cancelEditConfigValues(editConfigData.value);
}

async function onDelete() {
  const yes = await confirmDialog(
    `Do you want to delete '${data.value.config.configName}' config? (values will be preserved)`,
    'Delete Confirmation'
  );
  if (!yes) return;

  await deleteConfig(data.value.config);
}

async function onAddTarget(): Promise<void> {
  const res = await promptDialog(
    'Enter a new target name',
    'Adding a new target',
    (value?: string) => {
      if (!value) return 'Field is required';
      return isValidRedisKey(value) || 'Invalid format';
    }
  );
  if (!res) return;

  sortedEnvironments.value.forEach(env =>
    addConfigValue({
      configName: data.value.config.configName,
      groupName: data.value.config.groupName,
      environmentName: env,
      targetName: res
    })
  );

  expand(data.value.config.groupName, data.value.config.configName);
}
</script>

<template>
  <div class="flex flex-row-reverse align-items-center">
    <template v-if="['config', 'target'].includes(nodeType)">
      <EllipsisMenu
        :items="menuItems"
        :disabled="isEdit || nodeType === 'target'"
      />
      <Button
        v-if="!isEdit"
        @click="onEditNode"
        :disabled="selectedEnvironments.length == 0"
        icon="pi pi-pencil"
        size="small"
        severity="info"
        title="Edit values"
        class="flex-shrink-0"
        text
        rounded
      />
      <template v-else>
        <Button
          @click="onCancelEditNode"
          icon="pi pi-times"
          size="small"
          severity="warning"
          title="Cancel editing"
          class="flex-shrink-0"
          text
          rounded
        />
        <Button
          @click="onSaveNode"
          icon="pi pi-save"
          size="small"
          severity="success"
          class="flex-shrink-0"
          title="Save values"
          text
          rounded
        />
      </template>
      <i
        v-if="temporary"
        class="pi pi-exclamation-circle text-yellow-300 mr-4"
        title="Temporary, no values"
      />
    </template>
  </div>
</template>
