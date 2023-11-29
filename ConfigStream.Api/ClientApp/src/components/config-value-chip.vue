<script setup lang="ts">
import { computed } from 'vue';
const props = defineProps<{
  label: string | null | undefined;
}>();
const isPrimaryValue = computed(
  () =>
    !!props.label &&
    ['enable', 'enabled', 'on', 'true', 'yes'].includes(
      props.label.toLowerCase()
    )
);
</script>
<template>
  <Chip
    :label="props.label ?? undefined"
    :class="{ 'bg-primary': isPrimaryValue }"
  />
</template>
