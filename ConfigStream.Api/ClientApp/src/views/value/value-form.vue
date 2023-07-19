<template>
  <FormDialog
    class="config-form"
    v-model="dialog"
    :title="title"
    :loading="formDialog.loading"
    @save="onSave"
  >
    <div v-for="environment in environments" :key="environment">
      <v-select
        v-if="allowedValues.length > 0"
        v-model="data.values[environment]"
        :items="allowedValues"
        density="compact"
        variant="outlined"
        :label="environment"
        clearable
      ></v-select>
      <v-text-field
        v-else
        v-model="data.values[environment]"
        density="compact"
        variant="outlined"
        :label="environment"
        clearable
      ></v-text-field>
    </div>
  </FormDialog>
</template>
<script setup lang="ts">
import FormDialog from '@/components/form-dialog.vue';
import { useConfigValueStore } from '../../store/config-value';
import { reactive, computed, watch } from 'vue';

const data = reactive<{
  values: Record<string, string>;
}>({ values: {} });

const store = useConfigValueStore();
const formDialog = computed(() => store.formDialog);
const dialog = computed({
  get: () => formDialog.value.open,
  set: (v: boolean) => {
    if (!v) {
      store.closeDialog();
    }
  }
});
async function onSave() {
  store.submitConfigValue({
    configName: configValue.value?.configName ?? '',
    groupName: configValue.value?.groupName ?? '',
    environmentValues: data.values
  });
}

const configValue = computed(() => formDialog.value.configValue);
const title = computed(
  () =>
    `Edit '${configValue.value?.groupName}:${configValue.value?.configName}' values`
);
const environments = computed(() => Object.keys(data.values));
const allowedValues = computed(() =>
  configValue.value?.allowedValues &&
  configValue.value?.allowedValues.length > 0
    ? configValue.value?.allowedValues
    : []
);
watch(
  () => dialog.value,
  val => {
    if (!val) {
      return;
    }
    if (configValue.value) {
      data.values = {
        ...configValue.value.environmentValues
      };
    }
  }
);
</script>
