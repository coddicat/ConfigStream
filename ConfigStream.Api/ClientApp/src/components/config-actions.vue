<script setup lang="ts">
import { type ConfigValue, useConfigValueStore } from '@/store/config-value';
import { useConfigStore } from '@/store/config';
import { confirmDialog, promptDialog } from '@/utils/dialog';
import { isValidRedisKey } from '@/utils/redisKey';
import { type MenuItem } from 'primevue/menuitem';
import { TreeNode } from 'primevue/tree';
import { computed, nextTick, reactive } from 'vue';
import EllipsisMenu from './ellipsis-menu.vue';

const { submitConfigValues } = useConfigValueStore();
const { deleteConfig } = useConfigStore();

const emits = defineEmits<{
  (event: 'expand', configGroup: string, configName: string): void;
}>();

const props = defineProps<{
  node: TreeNode;
  editNodes: TreeNode[];
  environments: string[];
  configValues: ConfigValue[];
}>();

const _node = computed(() => props.node);
const _editNodes = computed(() => props.editNodes);
const _environments = computed(() => props.environments);
const _configValues = computed(() => props.configValues);

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

function onEditNode(node: TreeNode) {
  _editNodes.value.push(node);
  const json = JSON.stringify(node.data.configValues);
  node.data.originConfigValues = json;
}

function onCancelEditNode(node: TreeNode) {
  const index = _editNodes.value.indexOf(node);
  _editNodes.value.splice(index, 1);
  const origin = reactive(JSON.parse(node.data.originConfigValues));
  node.data.configValues = origin;
}

async function onSaveNode(node: TreeNode) {
  const index = _editNodes.value.indexOf(node);
  _editNodes.value.splice(index, 1);
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

  _environments.value.forEach(env =>
    _configValues.value.push({
      configName: node.data.config.name,
      groupName: node.data.config.groupName,
      environmentName: env,
      targetName: res
    })
  );

  nextTick(() => {
    emits('expand', node.data.config.groupName, node.data.config.name);
  });
}
</script>

<template>
  <div class="flex flex-row-reverse">
    <template v-if="['config', 'target'].includes(_node.type)">
      <EllipsisMenu
        :items="getMenuItems(node)"
        :disabled="editNodes.includes(node) || _node.type === 'target'"
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
