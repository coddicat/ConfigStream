<template>
  <v-app>
    <RouterView />
    <ConfirmDialog></ConfirmDialog>
    <PromptDialog></PromptDialog>
  </v-app>
</template>
<script setup lang="ts">
import ConfirmDialog from '@/components/confirm-dialog.vue';
import PromptDialog from '@/components/prompt-dialog.vue';
</script>
