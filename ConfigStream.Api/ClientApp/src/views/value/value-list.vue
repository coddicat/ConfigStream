<!-- eslint-disable vue/valid-v-slot -->
<template>
  <v-data-table
    :loading="loading"
    :headers="headers"
    :items="items"
    :items-length="total"
    :sort-by="!!sortBy ? [sortBy] : undefined"
    :page="page"
    :items-per-page="itemsPerPage"
    show-expand
    @update:page="updatePage"
    @update:items-per-page="updateItemsPerPage"
    :group-by="[{ key: 'groupName', order: 'asc' }]"
    density="compact"
    class="ma-2"
  >
    <template v-slot:top>
      <ValueListHeader></ValueListHeader>
    </template>

    <template
      v-for="env in selectedEnvironments"
      v-slot:[`item.environmentValues.${env}`]="{ item }"
    >
      <div :key="env">
        <div v-if="item.raw.environmentValues[env]">
          <ConfigValueChip
            :model-value="item.raw.environmentValues[env]"
            color="primary"
          ></ConfigValueChip>
        </div>
        <span v-else>-</span>
      </div>
    </template>
    <template v-slot:item.defaultValue="{ item }">
      <ConfigValueChip
        :model-value="item.raw.defaultValue"
        color="secondary"
      ></ConfigValueChip>
    </template>

    <template v-slot:item.actions="{ item }">
      <v-btn
        @click="onEdit(item.raw)"
        color="primary"
        variant="text"
        icon="mdi-pencil"
        density="compact"
      ></v-btn>
    </template>
  </v-data-table>
</template>
<script setup lang="ts">
import { VDataTable } from 'vuetify/labs/VDataTable';
import ConfigValueChip from '@/components/config-value-chip.vue';

import { computed, onMounted } from 'vue';
import { ConfigValue, useConfigValueStore } from '../../store/config-value';
import { storeToRefs } from 'pinia';
import ValueListHeader from './value-list-header.vue';
import { TableHeader } from '@/type';
import { useConfigEnvironmentStore } from '@/store/config-environment';
const store = useConfigValueStore();
const envStore = useConfigEnvironmentStore();
const { items: environments } = storeToRefs(envStore);
const { selectedEnvironments, items, itemsPerPage, loading, total, sortBy } =
  storeToRefs(store);
const { requestConfigValueList, updateItemsPerPage, updatePage } = store;

onMounted(async () => {
  await envStore.requestConfigEnvironments();
  selectedEnvironments.value =
    environments.value.length > 0 ? [environments.value[0].name] : [];
  requestConfigValueList();
});

const page = computed({
  get: () => store.page,
  set: (v: number) => store.updatePage(v)
});

const onEdit = (configValue: ConfigValue) => {
  store.openEditConfigDialog(configValue);
};

const headers = computed<TableHeader[]>(() => [
  {
    title: 'Config',
    key: 'configName',
    sortable: true
  },
  ...selectedEnvironments.value.map(
    (x): TableHeader => ({
      title: x + ' value',
      key: 'environmentValues.' + x
    })
  ),
  {
    title: 'Default value',
    key: 'defaultValue'
  },
  {
    title: '',
    key: 'actions',
    sortable: false,
    width: 115,
    align: 'center'
  }
]);
</script>
