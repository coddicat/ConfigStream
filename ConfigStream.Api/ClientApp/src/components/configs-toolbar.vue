<script setup lang="ts">
import { type Config, useConfigStore } from '@/store/config';
import { type ConfigValue } from '@/store/config-value';
import { promptDialog } from '@/utils/dialog';
import { computed, ref } from 'vue';
import SetupConfigDialog from './setup-config-dialog.vue';
const { createOrUpdateConfig } = useConfigStore();

const emits = defineEmits<{
  (event: 'expand', configGroup: string): void;
  (event: 'update:selectedEnvironments', environments: string[]): void;
}>();

const props = defineProps<{
  environments: string[];
  selectedEnvironments: string[];
  configValues: ConfigValue[];
  configs: Config[];
}>();

const _selectedEnvironments = computed({
  get: () => props.selectedEnvironments,
  set: (v: string[]) => emits('update:selectedEnvironments', v)
});
const _configValues = computed(() => props.configValues);
const setupConfig = ref(false);

async function addEnvironment() {
  const res = await promptDialog(
    'Enter a new environment name',
    'Adding a new environment'
  );
  if (!res) return;
  _configValues.value.push({
    configName: props.configs[0].name,
    groupName: props.configs[0].groupName,
    environmentName: res
  });
  _selectedEnvironments.value = [..._selectedEnvironments.value, res];
}

async function onSubmitSetupConfig(config: Config) {
  await createOrUpdateConfig(config);
  emits('expand', config.groupName);
}
</script>
<template>
  <SetupConfigDialog v-model="setupConfig" @submit="onSubmitSetupConfig" />
  <Toolbar class="border-none p-0">
    <!-- <template #center>
            <div class="p-input-icon-left">
              <i class="pi pi-search"></i>
              <InputText placeholder="Config name" :model-value="search" />
            </div>
          </template> -->
    <template #start>
      <Button
        @click="setupConfig = true"
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
        v-model="_selectedEnvironments"
        :options="environments"
        class="w-full sm:w-26rem"
        display="chip"
      >
      </MultiSelect>
    </template>
  </Toolbar>
</template>
