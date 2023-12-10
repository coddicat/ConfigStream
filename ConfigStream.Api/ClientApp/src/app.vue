<script setup lang="ts">
import ConfirmDialog from '@/components/confirm-dialog.vue';
import PromptDialog from '@/components/prompt-dialog.vue';
import { useHomeStore } from '@/store/home';
import { useToast } from 'primevue/usetoast';
const toast = useToast();
const homeStore = useHomeStore();
homeStore.initToast(toast);
const version = homeStore.version;
</script>

<template>
  <Toolbar class="border-noround border-none mb-2 surface-50">
    <template #start>
      <span class="text-xl">ConfigStream v{{ version }}</span>
    </template>
  </Toolbar>
  <div class="border-1 surface-border m-2">
    <RouterView />
    <ConfirmDialog />
    <PromptDialog />
    <Toast position="bottom-center" />
  </div>
</template>
