<script setup lang="ts">
import { promptDialog } from '@/utils/dialog';
import { computed } from 'vue';
import { storeToRefs } from 'pinia';
import { useHomeStore } from '@/store/home';
const homeStore = useHomeStore();
const { sortedEnvironments, sortedSelectedEnvironments, disabledEnvironments } =
  storeToRefs(homeStore);
const { addEnvironment, openSetupConfigDialog } = homeStore;

async function onAddEnvironment() {
  const res = await promptDialog(
    'Enter a new environment name',
    'Adding a new environment'
  );
  if (!res) return;
  addEnvironment(res);
}

const selectedEnvironments = computed({
  get: () => sortedSelectedEnvironments.value,
  set: v => (homeStore.selectedEnvironments = v)
});
</script>
<template>
  <Toolbar class="border-none p-0">
    <!-- <template #center>
      <Dropdown
        v-model="selectedClient"
        editable
        :options="['CLEINT1', 'CLEINT2', 'CLEINT3']"
        placeholder="Select a City"
        class="w-full md:w-14rem"
      />
      {{ selectedClient }}
    </template> -->
    <template #start>
      <Button
        @click="openSetupConfigDialog()"
        label="New config"
        icon="pi pi-plus"
        size="small"
      ></Button>
    </template>
    <template #end>
      <Button
        @click="onAddEnvironment"
        :disabled="disabledEnvironments"
        label="New environment"
        size="small"
        icon="pi pi-plus"
        class="mr-3"
      ></Button>
      <MultiSelect
        v-model="selectedEnvironments"
        :options="sortedEnvironments"
        :disabled="disabledEnvironments"
        class="w-full sm:w-26rem"
        display="chip"
      >
      </MultiSelect>
    </template>
  </Toolbar>
</template>
