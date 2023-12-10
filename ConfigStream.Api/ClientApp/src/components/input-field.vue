<script setup lang="ts">
defineProps<{
  label: string;
  modelValue: string | undefined;
  error?: string | boolean;
  disabled?: boolean;
}>();
defineEmits<{
  (event: 'update:modelValue', value: string): void;
}>();
</script>

<template>
  <div class="flex flex-column gap-1 mt-4">
    <span class="p-float-label">
      <InputText
        type="text"
        :model-value="modelValue"
        :disabled="disabled"
        @update:model-value="(v: string) => $emit('update:modelValue', v)"
        :class="{ 'p-invalid': error }"
      />
      <label>{{ label }}</label>
    </span>
    <small v-if="error" class="p-error">{{ error }}</small>
  </div>
</template>
