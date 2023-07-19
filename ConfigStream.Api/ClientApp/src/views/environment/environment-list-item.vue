<template>
  <div>
    <v-list-item :title="modelValue.name">
      <template v-slot:append>
        <v-btn
          size="x-small"
          icon="mdi-delete"
          color="error"
          variant="outlined"
          @click.stop="onDelete()"
        >
        </v-btn>
      </template>
    </v-list-item>
    <v-divider></v-divider>
  </div>
</template>
<script setup lang="ts">
import {
  ConfigEnvironment,
  useConfigEnvironmentStore
} from '@/store/config-environment';
import { confirmDialog } from '@/utils/dialog';

const { deleteConfigEnvironment } = useConfigEnvironmentStore();
const props = defineProps<{
  modelValue: ConfigEnvironment;
}>();

async function onDelete() {
  const response = await confirmDialog(
    `Are you sure want to delete '${props.modelValue.name}' environment?`
  );
  if (!response) {
    return;
  }

  await deleteConfigEnvironment(props.modelValue);
}
</script>
