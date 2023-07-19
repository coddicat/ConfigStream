<template>
  <v-dialog v-model="dialog" max-width="500" persistent>
    <v-card>
      <v-card-title v-if="model?.title">{{ model?.title }}</v-card-title>
      <v-card-text>
        <div>{{ model?.message }}</div>
        <v-form v-model="isValid" ref="form" @submit="onSubmit">
          <v-text-field
            ref="inputField"
            variant="outlined"
            density="compact"
            v-model="input"
            :rules="model?.rules"
          ></v-text-field>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="warning" @click="onCancel"> Cancel </v-btn>
        <v-btn color="primary" @click="onSubmit" :disabled="!isValid">
          Submit
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
<script setup lang="ts">
import { useDialogStore } from '@/store/dialog';
import { watch, ref, computed } from 'vue';

const form = ref<HTMLFormElement>();
const isValid = ref<boolean>();
const store = useDialogStore();
const model = computed(() => store.promptDialog);
const input = ref<string>();
const inputField = ref<HTMLInputElement>();
const dialog = computed({
  get: () => model.value?.dialog ?? false,
  set: (v: boolean) => {
    if (model.value?.dialog) {
      model.value.dialog = v;
    }
  }
});

async function onSubmit(e: Event) {
  e.preventDefault();

  await form.value?.validate();
  if (!isValid.value) {
    return;
  }

  model.value?.resolve(input.value);
  dialog.value = false;
}

function onCancel() {
  model.value?.resolve(undefined);
  dialog.value = false;
}

watch(
  () => dialog.value,
  val => {
    if (val) {
      setTimeout(() => {
        inputField.value?.focus();
      }, 300);
    } else {
      form.value?.reset();
    }
  }
);
</script>
