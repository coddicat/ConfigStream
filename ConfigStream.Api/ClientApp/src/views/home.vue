<script setup lang="ts">
import { useConfigStore } from '@/store/config';
import { type ConfigValue, useConfigValueStore } from '@/store/config-value';
import { useHomeStore } from '@/store/home';
import { storeToRefs } from 'pinia';
import type { TreeNode } from 'primevue/tree';
import { onMounted, computed, reactive } from 'vue';
import { groupBy } from 'lodash';
import ConfigValueComponent from '@/components/config-value.vue';
import ConfigValueChip from '@/components/config-value-chip.vue';
import ConfigActions from '@/components/config-actions.vue';
import ConfigsToolbar from '@/components/configs-toolbar.vue';
import SetupConfigDialog from '@/components/setup-config-dialog.vue';
import { sort } from '@/utils/array';
import LoadingSpinner from '@/components/loading-spinner.vue';

const configStore = useConfigStore();
const configValueStore = useConfigValueStore();
const homeStore = useHomeStore();

const { configValues } = storeToRefs(configValueStore);
const { configs } = storeToRefs(configStore);
const { sortedSelectedEnvironments, expandedNodes, loading } =
  storeToRefs(homeStore);
const { load, isEditing } = homeStore;

onMounted(async () => {
  await load();
});

const nodes = computed<TreeNode[]>(() => {
  if (!configs.value || !configValues.value) {
    return [];
  }

  const noValue = configs.value
    .filter(
      x =>
        !configValues.value.some(
          y => x.groupName === y.groupName && x.configName === y.configName
        )
    )
    .map(x => ({
      groupName: x.groupName,
      configName: x.configName,
      environmentName: 'undefined'
    }));

  const list = sort(
    [...configValues.value, ...noValue],
    x => `${x.groupName}:${x.configName}`
  );
  return groupByGroupName(list);
});

function getConfig(groupName: string, configName: string) {
  return (
    configs.value.find(
      y => y.groupName === groupName && y.configName === configName
    ) ?? reactive({ groupName, configName, deleted: true })
  );
}

function getEnvironmentValues(
  groupName: string,
  configName: string,
  targetName: string | undefined,
  values: ConfigValue[]
) {
  const vals: Record<string, ConfigValue> = {};
  sortedSelectedEnvironments.value.forEach(env => {
    let value = values.find(x => x.environmentName === env);
    if (!value) {
      value = reactive({
        configName,
        groupName,
        environmentName: env,
        targetName,
        value: undefined
      });
      configValues.value.push(value);
    }
    vals[env] = value;
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

function isEdit(node: TreeNode): boolean {
  return isEditing({
    configName: node.data.config.configName,
    groupName: node.data.config.groupName,
    targetName: node.type === 'target' ? node.label : undefined
  });
}
</script>

<template>
  <main>
    <TreeTable
      :value="nodes"
      v-model:expanded-keys="expandedNodes"
      class="p-treetable-sm"
      resizable-columns
      show-gridlines
    >
      <template #header>
        <ConfigsToolbar />
      </template>

      <Column header="Name" expander body-class="home__ellipsis-column">
        <template #body="{ node }">
          <i v-if="node.type === 'target'" class="pi pi-stop-circle mr-2" />
          <span v-if="node.data?.config.deleted" class="text-yellow-300 mr-2">
            (deleted)
          </span>
          {{ node.label }}
        </template>
      </Column>
      <Column header="Description" body-class="home__ellipsis-column">
        <template #body="{ node }">
          <span v-if="node.type === 'config'">
            {{ node.data.config.description }}
          </span>
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
        v-for="env in sortedSelectedEnvironments"
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
            :edit="isEdit(node)"
          />
        </template>
      </Column>
      <Column headerStyle="width: 9.5rem">
        <template #body="{ node }">
          <ConfigActions :node="node" />
        </template>
      </Column>
    </TreeTable>

    <SetupConfigDialog />
    <LoadingSpinner :value="loading" />
  </main>
</template>

<style lang="scss">
.home {
  &__ellipsis-column {
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }
}
</style>
