<template>
  <Dialog
    v-model:visible="dialog"
    style="max-width: 40rem"
    modal
    :header="model?.title"
  >
    <p class="m-0">{{ model?.message }}</p>
    <form ref="form" @submit="onSubmit" class="flex flex-column gap-2">
      <InputText
        type="text"
        v-model="value"
        autofocus
        :class="{ 'p-invalid': errorMessage }"
      />
      <small class="p-error">{{ errorMessage || '&nbsp;' }}</small>
    </form>
    <template #footer>
      <Button severity="secondary" @click="onCancel" label="Cancel" />
      <Button
        severity="primary"
        @click="onSubmit"
        :disabled="!meta.valid"
        label="Submit"
      />
    </template>
  </Dialog>
</template>
<script setup lang="ts">
import { useDialogStore } from '@/store/dialog';
import { watch, computed } from 'vue';
import { useField, useForm } from 'vee-validate';

const { handleSubmit, resetForm, meta } = useForm();
const { value, errorMessage } = useField<string>('value', validateField);

function validateField(value: string | undefined): string | boolean {
  if (model.value?.validate) return model.value.validate(value);
  return !!value || 'Field is required';
}

const store = useDialogStore();
const model = computed(() => store.promptDialog);
const dialog = computed({
  get: () => model.value?.dialog ?? false,
  set: (v: boolean) => {
    if (model.value?.dialog) {
      model.value.dialog = v;
    }
  }
});

const onSubmit = handleSubmit(values => {
  model.value?.resolve(values.value);
  dialog.value = false;
});

function onCancel() {
  model.value?.resolve(undefined);
  dialog.value = false;
}

watch(
  () => dialog.value,
  () => resetForm()
);
</script>
