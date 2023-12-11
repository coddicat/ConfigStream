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
      <img src="@/assets/logo.png" width="32" height="32" class="mr-2" />
      <span class="text-xl">ConfigStream</span>
      <span class="text-base text-primary ml-2">v{{ version }}</span>
    </template>
  </Toolbar>
  <div class="border-1 surface-border m-2">
    <RouterView />
    <ConfirmDialog />
    <PromptDialog />
    <Toast position="bottom-center" />
  </div>
</template>
