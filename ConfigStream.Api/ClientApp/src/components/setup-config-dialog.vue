<template>
  <Dialog
    v-model:visible="visible"
    style="max-width: 50rem"
    modal
    header="Setup config"
  >
    <form ref="form" @submit="onSubmit" class="flex flex-column">
      <div class="flex flex-row gap-3">
        <InputField
          v-model="groupName"
          :error="errors.groupName"
          label="Group name"
        />
        <InputField
          v-model="configName"
          :error="errors.configName"
          label="Config name"
        />
      </div>
      <div class="flex flex-row gap-3 mt-2">
        <InputField v-model="description" label="Description" />
        <InputField
          v-model="defaultValue"
          :error="errors.defaultValue"
          label="Default value"
        />
      </div>

      <div class="flex flex-column gap-1 mt-5 p-fluid">
        <span class="p-float-label">
          <Chips type="text" v-model="allowedValues" separator=" " />
          <label>Allowed values</label>
        </span>
      </div>
    </form>
    <template #footer>
      <Button severity="secondary" @click="onCancel" label="Cancel" />
      <Button
        severity="primary"
        @click="onSubmit"
        :disabled="!meta.valid"
        label="Save"
      />
    </template>
  </Dialog>
</template>
<script setup lang="ts">
import { computed, watch } from 'vue';
import { useField, useForm } from 'vee-validate';
import { isValidRedisKey } from '@/utils/redisKey';
import InputField from './input-field.vue';
import { useConfigStore, type Config } from '@/store/config';
import { useHomeStore } from '@/store/home';
import { storeToRefs } from 'pinia';
const homeStore = useHomeStore();
const { closeSetupConfigDialog, expand } = homeStore;
const { createOrUpdateConfig } = useConfigStore();
const { setupConfigDialog } = storeToRefs(homeStore);
const { handleSubmit, resetForm, errors, meta } = useForm();
const { value: groupName } = useField<string>('groupName', validateKey);
const { value: configName } = useField<string>('configName', validateKey);
const { value: description } = useField<string>('description');
const { value: defaultValue, validate: validateDefaultValue } =
  useField<string>('defaultValue', (value: string) => {
    if (!value || !allowedValues.value || allowedValues.value.length === 0) {
      return true;
    }
    return allowedValues.value.includes(value) || 'Must match an allowed value';
  });
const { value: allowedValues } = useField<string[]>('allowedValues');

function validateKey(value: string): boolean | string {
  if (!isValidRedisKey(value)) return 'Invalid format';
  return !!value || 'Field is required';
}

const visible = computed({
  get: () => !!setupConfigDialog.value,
  set: v => {
    if (!v) onCancel();
  }
});

watch(
  () => allowedValues.value,
  () => validateDefaultValue()
);

const onSubmit = handleSubmit(values => {
  createOrUpdateConfig({
    groupName: values.groupName,
    name: values.configName,
    description: values.description,
    defaultValue: values.defaultValue,
    allowedValues: values.allowedValues
  });
  expand(values.groupName);
  onCancel();
});

function onCancel() {
  closeSetupConfigDialog();
}

watch(
  () => setupConfigDialog.value,
  (model: boolean | Config) => {
    resetForm();
    if (typeof model == 'boolean') {
      return;
    }
    groupName.value = model.groupName;
    configName.value = model.name;
    description.value = model.description;
    defaultValue.value = model.defaultValue;
    allowedValues.value = model.allowedValues;
  }
);
</script>
