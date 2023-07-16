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
import { ConfigGroup, useConfigGroupStore } from '@/store/config-group';
import { confirmDialog } from '@/utils/dialog';

const { deleteConfigGroup } = useConfigGroupStore();
const props = defineProps<{
  modelValue: ConfigGroup;
}>();

async function onDelete() {
  const response = await confirmDialog(
    `Are you sure want to delete '${props.modelValue.name}' group?`
  );
  if (!response) {
    return;
  }

  await deleteConfigGroup(props.modelValue);
}
</script>
