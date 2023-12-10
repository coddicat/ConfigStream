<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
  label: string | null | undefined;
}>();

const primaryValues = ['enable', 'enabled', 'on', 'true', 'yes'];

const isPrimaryValue = computed(
  () => !!props.label && primaryValues.includes(props.label.toLowerCase())
);
</script>

<template>
  <Chip
    :label="props.label ?? undefined"
    :class="{ 'bg-primary': isPrimaryValue }"
  />
</template>
