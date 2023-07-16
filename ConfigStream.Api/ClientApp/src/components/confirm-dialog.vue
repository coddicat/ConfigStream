<template>
  <v-dialog v-model="dialog" max-width="500" persistent>
    <v-card>
      <v-card-title v-if="model?.title">{{ model?.title }}</v-card-title>
      <v-card-text>{{ model?.message }}</v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="warning" @click="onDisagree"> Disagree </v-btn>
        <v-btn color="primary" @click="onAgree"> Agree </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script setup lang="ts">
import { useDialogStore } from '@/store/dialog';
import { computed } from 'vue';
const store = useDialogStore();
const model = computed(() => store.confirmDialog);

const dialog = computed({
  get: () => model.value?.dialog ?? false,
  set: (v: boolean) => {
    if (model.value?.dialog) {
      model.value.dialog = v;
    }
  }
});
function onAgree() {
  model.value?.resolve(true);
  dialog.value = false;
}
function onDisagree() {
  model.value?.resolve(false);
  dialog.value = false;
}
</script>
