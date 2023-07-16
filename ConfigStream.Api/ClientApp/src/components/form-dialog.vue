<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="v => $emit('update:modelValue', v)"
    max-width="600"
    persistent
  >
    <v-card :loading="loading" :disabled="loading">
      <v-card-title>{{ title }}</v-card-title>
      <v-card-text>
        <v-form ref="form">
          <slot></slot>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="default" @click="onCancel">
          {{ cancelText ?? 'Cancel' }}
        </v-btn>
        <v-btn color="primary" @click="onSave">
          {{ saveText ?? 'Save' }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script setup lang="ts">
import { watch, ref } from 'vue';

const props = withDefaults(
  defineProps<{
    modelValue: boolean;
    title: string;
    cancelText?: string;
    saveText?: string;
    loading?: boolean;
  }>(),
  {
    cancelText: 'Cancel',
    saveText: 'Save',
    loading: false
  }
);
const emit = defineEmits<{
  (event: 'update:modelValue', value: boolean): void;
  (event: 'save'): void;
}>();
const form = ref<HTMLFormElement>();
async function onSave() {
  if (!form.value) {
    return;
  }
  const validation = await form.value.validate();
  const valid = validation.valid;
  if (!valid) {
    return;
  }
  emit('save');
}
function onCancel() {
  emit('update:modelValue', false);
}

watch(
  () => props.modelValue,
  () => form.value?.reset()
);
</script>
