<script setup lang="ts">
import ConfirmDialog from '@/components/confirm-dialog.vue';
import PromptDialog from '@/components/prompt-dialog.vue';
</script>
<template>
  <div class="border-1 surface-border">
    <RouterView></RouterView>
    <ConfirmDialog></ConfirmDialog>
    <PromptDialog></PromptDialog>
  </div>
</template>
