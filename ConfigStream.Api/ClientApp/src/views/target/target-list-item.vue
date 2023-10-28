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
import { ConfigTarget, useConfigTargetStore } from '@/store/config-target';
import { confirmDialog } from '@/utils/dialog';

const { deleteConfigTarget } = useConfigTargetStore();
const props = defineProps<{
  modelValue: ConfigTarget;
}>();

async function onDelete() {
  const response = await confirmDialog(
    `Are you sure want to delete '${props.modelValue.name}' target?`
  );
  if (!response) {
    return;
  }

  await deleteConfigTarget(props.modelValue);
}
</script>
