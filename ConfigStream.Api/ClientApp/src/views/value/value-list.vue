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
    @update:page="updatePage"
    @update:items-per-page="updateItemsPerPage"
    :group-by="[{ key: 'groupName', order: 'asc' }]"
    density="compact"
    class="ma-2"
  >
    <!-- <template v-slot:top>
      <ConfigListHeader></ConfigListHeader>
    </template> -->
    <template v-slot:item.actions="{ item }">
      <v-btn
        @click="onEdit(item.raw)"
        color="primary"
        variant="text"
        icon="mdi-pencil"
        density="compact"
      ></v-btn>
      <v-btn
        @click="onDelete(item.raw)"
        color="error"
        variant="text"
        icon="mdi-delete"
        density="compact"
      ></v-btn>
    </template>
  </v-data-table>
</template>
<script setup lang="ts">
import { VDataTable } from 'vuetify/labs/VDataTable';
import { computed, onMounted } from 'vue';
import {
  useConfigValueStore,
  type ConfigValue
} from '../../store/config-value';
import { storeToRefs } from 'pinia';
import { confirmDialog } from '@/utils/dialog';
// import ConfigListHeader from './config-list-header.vue';
import { TableHeader } from '@/type';
const store = useConfigValueStore();
const { items, itemsPerPage, loading, total, sortBy } = storeToRefs(store);
const { updateItemsPerPage, updatePage } = store;

onMounted(() => {
  store.requestConfigValueList();
});

const page = computed({
  get: () => store.page,
  set: (v: number) => store.updatePage(v)
});

const onEdit = (configValue: ConfigValue) => {
  console.log(configValue);
}; // store.openEditConfigDialog;
async function onDelete(configValue: ConfigValue) {
  const response = await confirmDialog(
    `Are you sure want to reset '${configValue.groupName}:${configValue.configName}'?`
  );
  if (!response) {
    return;
  }
  store.resetConfigValue(configValue);
}

const headers: TableHeader[] = [
  {
    title: 'Config name',
    key: 'configName',
    sortable: true
  },
  {
    title: 'Value',
    key: 'value'
  },
  {
    title: 'DefaultValue',
    key: 'defaultValue'
  },
  {
    title: '',
    key: 'actions',
    sortable: false,
    width: 115,
    align: 'center'
  }
];
</script>
