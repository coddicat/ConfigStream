<script setup lang="ts">
import { nextTick } from 'process';
import { ref, watch } from 'vue';

const props = defineProps<{
  value: boolean;
}>();
const visible = ref(props.value);
let stop: NodeJS.Timeout | undefined = undefined;
let start: Date | undefined = props.value ? new Date() : undefined;

watch(
  () => props.value,
  nv => {
    const now = new Date();
    clearTimeout(stop);
    if (nv) {
      start = now;
      visible.value = true;
      return;
    }

    const delay = 200 - (now.getTime() - (start ?? now).getTime());
    stop = setTimeout(() => {
      nextTick(() => {
        visible.value = false;
      });
    }, delay);
  }
);
</script>

<template>
  <div v-show="visible" class="loading-spinner bg-white-alpha-20">
    <ProgressSpinner strokeWidth="4" />
    <span>Hold on...</span>
  </div>
</template>

<style lang="scss">
.loading-spinner {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1000;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
}
</style>
