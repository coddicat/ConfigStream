<template>
  <Dialog
    v-model:visible="dialog"
    style="max-width: 40rem"
    modal
    :header="model?.title"
  >
    <p class="m-0">{{ model?.message }}</p>
    <template #footer>
      <Button severity="success" @click="onAgree" label="Agree" />
      <Button
        severity="secondary"
        @click="onDisagree"
        label="Disagree"
        autofocus
      />
    </template>
  </Dialog>
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
