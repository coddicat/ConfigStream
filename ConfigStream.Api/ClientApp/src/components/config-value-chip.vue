<template>
  <v-chip v-if="modelValue" :color="color" density="comfortable">
    {{ modelValue }}
  </v-chip>
  <span v-else>-</span>
</template>
<script setup lang="ts">
defineProps<{
  modelValue?: string;
  color: string;
}>();
</script>
