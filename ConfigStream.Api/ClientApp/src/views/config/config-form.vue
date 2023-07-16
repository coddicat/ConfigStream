<template>
  <FormDialog
    class="config-form"
    v-model="dialog"
    :title="formTitle"
    :loading="formDialog.loading"
    @save="onSave"
  >
    <v-combobox
      v-model="data.groupName"
      :items="groupItems"
      item-title="name"
      item-value="name"
      density="compact"
      variant="outlined"
      clearable
      label="Group*"
      :rules="fieldRules.groupName"
      :loading="groupLoading"
      :disabled="groupLoading"
    >
      <template v-slot:append v-if="newGroup">
        <v-tooltip text="New group name will be added">
          <template v-slot:activator="{ props }">
            <v-icon v-bind="props" color="warning">mdi-alert-circle</v-icon>
          </template>
        </v-tooltip>
      </template>
    </v-combobox>
    <v-text-field
      v-model="data.name"
      label="Name*"
      density="compact"
      variant="outlined"
      :rules="fieldRules.configName"
      :disabled="!formDialog.new"
    ></v-text-field>
    <v-combobox
      v-model="data.allowedValues"
      label="Allowed values"
      :delimiters="[' ', ',', ';']"
      chips
      closable-chips
      multiple
      clearable
      density="compact"
      variant="outlined"
    ></v-combobox>
    <v-text-field
      v-model="data.defaultValue"
      label="Default Value"
      density="compact"
      variant="outlined"
    ></v-text-field>
    <v-textarea
      v-model="data.description"
      label="Description"
      density="compact"
      variant="outlined"
      rows="1"
    ></v-textarea>
  </FormDialog>
</template>
<script setup lang="ts">
import FormDialog from '@/components/form-dialog.vue';
import { useConfigStore, type Config } from '../../store/config';
import { useConfigGroupStore } from '../../store/config-group';
import { reactive, computed, watch } from 'vue';
import { redisKeyRule, requiredRule } from '@/input-rules';

const data = reactive<Config>({
  allowedValues: [],
  groupName: '',
  name: ''
});
const configStore = useConfigStore();
const configGroupStore = useConfigGroupStore();
const groupLoading = computed(() => configGroupStore.loading);
const groupItems = computed(() => configGroupStore.items.map(x => x.name));
const formDialog = computed(() => configStore.formDialog);
const formTitle = computed(() =>
  formDialog.value?.new ? 'Create config' : 'Edit config'
);
const dialog = computed({
  get: () => formDialog.value.open,
  set: (v: boolean) => {
    if (!v) {
      configStore.closeDialog();
    }
  }
});

const newGroup = computed(() => {
  if ((data.groupName?.trim() ?? '').length === 0) {
    return false;
  }
  const exists = groupItems.value.find(x => x === data.groupName.trim());
  return !exists;
});

const fieldRules = {
  configName: [requiredRule, redisKeyRule],
  groupName: [requiredRule, redisKeyRule]
};
async function onSave() {
  configStore.createOrUpdateConfig(data, newGroup.value);
}

watch(
  () => dialog.value,
  val => {
    if (!val) {
      return;
    }
    if (formDialog.value.config) {
      data.allowedValues = formDialog.value.config.allowedValues;
      data.defaultValue = formDialog.value.config.defaultValue;
      data.description = formDialog.value.config.description;
      data.groupName = formDialog.value.config.groupName;
      data.name = formDialog.value.config.name;
    }
    configGroupStore.requestConfigGroups();
  }
);
</script>
