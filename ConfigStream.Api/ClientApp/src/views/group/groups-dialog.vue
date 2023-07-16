<template>
  <v-dialog v-model="model" width="600" class="groups-dialog">
    <v-card>
      <v-card-title>Config Groups</v-card-title>
      <v-form ref="form">
        <v-card-actions>
          <v-btn
            @click="onRefresh"
            class="mr-2"
            icon="mdi-refresh"
            variant="flat"
          ></v-btn>
          <v-btn
            @click="onNewGroup"
            variant="outlined"
            color="primary"
            prepend-icon="mdi-plus"
          >
            New group
          </v-btn>
          <v-spacer></v-spacer>
          <v-text-field
            v-model="search"
            @update:model-value="onSearch"
            label="Search"
            density="compact"
            variant="outlined"
            append-inner-icon="mdi-magnify"
            hide-details
            clearable
            class="py-2"
          >
          </v-text-field>
        </v-card-actions>
      </v-form>
      <v-card
        :loading="loading"
        :disabled="loading"
        class="mx-2 groups-dialog__list"
        variant="outlined"
        height="350"
      >
        <v-list v-if="items?.length > 0" density="compact">
          <group-list-item
            :model-value="item"
            v-for="item in items"
            :key="item.name"
          ></group-list-item>
        </v-list>
        <v-card-text v-else>
          Group list is empty. Click 'New Group' button to add a new group
        </v-card-text>
      </v-card>
      <v-card-actions>
        <v-spacer></v-spacer>
        <v-btn color="warning" @click="onClose">Close</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, watch, ref } from 'vue';
import { useConfigGroupStore } from '@/store/config-group';
import { storeToRefs } from 'pinia';
import GroupListItem from './group-list-item.vue';
import { promptDialog } from '@/utils/dialog';
import { debounce } from 'lodash';
import { redisKeyRule, requiredRule } from '@/input-rules';

const store = useConfigGroupStore();
const { items, loading, search } = storeToRefs(store);
const { requestConfigGroups, createConfigGroup } = store;
const form = ref<HTMLFormElement>();

const props = defineProps<{
  modelValue: boolean;
}>();

const emits = defineEmits<{
  (event: 'update:modelValue', value: boolean): void;
}>();

const model = computed({
  get: () => props.modelValue,
  set: (v: boolean) => emits('update:modelValue', v)
});

function onClose() {
  model.value = false;
}

async function onNewGroup() {
  const rules = [requiredRule, redisKeyRule];
  const result = await promptDialog('Enter group name:', undefined, rules);
  if (result) {
    createConfigGroup({
      name: result
    });
  }
}

function onRefresh() {
  if (props.modelValue) {
    return requestConfigGroups();
  }
}

const onSearch = debounce(onRefresh, 300);

watch(
  () => props.modelValue,
  async val => {
    if (val) {
      await requestConfigGroups();
    } else {
      form.value?.reset();
    }
  }
);
</script>

<style>
.groups-dialog {
  .groups-dialog__list {
    overflow-y: auto;
  }
}
</style>
