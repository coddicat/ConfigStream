<script setup lang="ts">
import ConfirmDialog from '@/components/confirm-dialog.vue';
import PromptDialog from '@/components/prompt-dialog.vue';
</script>
<template>
  <Toolbar class="border-noround border-none mb-2 bg-primary">
    <template #start>
      <span>ConfigStream v0.0.1</span>
    </template>
  </Toolbar>
  <div class="border-1 surface-border m-2">
    <RouterView></RouterView>
    <ConfirmDialog></ConfirmDialog>
    <PromptDialog></PromptDialog>
  </div>
</template>
