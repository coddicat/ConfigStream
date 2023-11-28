<template>
  <Dialog
    :visible="modelValue"
    @update:visible="v => $emit('update:modelValue', v)"
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
import { watch } from 'vue';
import { useField, useForm } from 'vee-validate';
import { isValidRedisKey } from '@/utils/redisKey';
import InputField from './input-field.vue';
import { Config } from '@/store/config';

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

watch(
  () => allowedValues.value,
  () => validateDefaultValue()
);

const props = defineProps<{
  modelValue: boolean;
}>();

const emit = defineEmits<{
  (event: 'update:modelValue', value: boolean): void;
  (event: 'submit', value: Config): void;
}>();

const onSubmit = handleSubmit(values => {
  emit('submit', {
    groupName: values.groupName,
    name: values.configName,
    description: values.description,
    defaultValue: values.defaultValue,
    allowedValues: values.allowedValues
  });
  onCancel();
});

function onCancel() {
  emit('update:modelValue', false);
}

watch(
  () => props.modelValue,
  () => resetForm()
);
</script>
