<script setup lang="ts">
import { promptDialog } from '@/utils/dialog';
import { computed } from 'vue';
import { storeToRefs } from 'pinia';
import { useHomeStore } from '@/store/home';
import { useConfigValueStore } from '@/store/config-value';
const homeStore = useHomeStore();
const configValuesStore = useConfigValueStore();
const {
  selectedEnvironments,
  sortedEnvironments,
  sortedSelectedEnvironments,
  disabledEnvironments
} = storeToRefs(homeStore);
const { configValues } = storeToRefs(configValuesStore);
const { load, addEnvironment, openSetupConfigDialog } = homeStore;
async function onAddEnvironment() {
  const res = await promptDialog(
    'Enter a new environment name',
    'Adding a new environment'
  );
  if (!res) return;
  addEnvironment(res);
}
const environments = computed({
  get: () => sortedSelectedEnvironments.value,
  set: v => (selectedEnvironments.value = v)
});
function isTemporaryEnvironment(environment: string): boolean {
  return !configValues.value.some(
    x =>
      x.environmentName === environment &&
      x.value !== undefined &&
      x.value !== null
  );
}
</script>

<template>
  <Toolbar class="border-none p-0">
    <template #center>
      <Button icon="pi pi-refresh" text rounded @click="load()" />
    </template>
    <template #start>
      <Button
        @click="openSetupConfigDialog()"
        label="New config"
        icon="pi pi-plus"
        size="small"
      />
    </template>
    <template #end>
      <Button
        @click="onAddEnvironment"
        :disabled="disabledEnvironments"
        label="New environment"
        size="small"
        icon="pi pi-plus"
        class="mr-3"
      />
      <MultiSelect
        v-model="environments"
        :options="sortedEnvironments"
        :disabled="disabledEnvironments"
        class="w-full sm:w-26rem"
        display="chip"
      >
        <template #chip="{ value }">
          <i
            v-if="isTemporaryEnvironment(value)"
            class="pi pi-exclamation-circle text-yellow-300 mr-2"
            title="Temporary, no values"
          />

          {{ value }}
        </template>
      </MultiSelect>
    </template>
  </Toolbar>
</template>
