<script setup lang="ts">
import { computed } from 'vue';
const props = defineProps<{
  label: string | null | undefined;
}>();
const isPrimaryValue = computed(() =>
  ['enable', 'enabled', 'on', 'true', 'yes'].includes(
    props.label?.toLowerCase()
  )
);
</script>
<template>
  <Chip :label="props.label" :class="{ 'bg-primary': isPrimaryValue }" />
</template>
