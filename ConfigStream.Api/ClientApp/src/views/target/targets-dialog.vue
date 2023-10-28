<template>
  <v-dialog v-model="model" width="600" class="targets-dialog">
    <v-card>
      <v-card-title>Config Targets</v-card-title>
      <v-form ref="form" @submit="preventSubmit">
        <v-card-actions>
          <v-btn
            @click="onRefresh"
            class="mr-2"
            icon="mdi-refresh"
            variant="flat"
          ></v-btn>
          <v-btn
            @click="onNewTarget"
            variant="outlined"
            color="primary"
            prepend-icon="mdi-plus"
          >
            New target
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
        class="mx-2 targets-dialog__list"
        variant="outlined"
        height="350"
      >
        <v-list v-if="items?.length > 0" density="compact">
          <target-list-item
            :model-value="item"
            v-for="item in items"
            :key="item.name"
          ></target-list-item>
        </v-list>
        <v-card-text v-else>
          Target list is empty. Click 'New Target' button to add a new target
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
import { useConfigTargetStore } from '@/store/config-target';
import { storeToRefs } from 'pinia';
import TargetListItem from './target-list-item.vue';
import { promptDialog } from '@/utils/dialog';
import { debounce } from 'lodash';
import { redisKeyRule, requiredRule } from '@/input-rules';

const store = useConfigTargetStore();
const { items, loading, search } = storeToRefs(store);
const { requestConfigTargets, createConfigTarget } = store;
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

function preventSubmit(e: Event) {
  e.preventDefault();
}

async function onNewTarget() {
  const rules = [requiredRule, redisKeyRule];
  const result = await promptDialog('Enter target name:', undefined, rules);
  if (result) {
    createConfigTarget({
      name: result
    });
  }
}

function onRefresh() {
  if (props.modelValue) {
    return requestConfigTargets();
  }
}

const onSearch = debounce(onRefresh, 300);

watch(
  () => props.modelValue,
  async val => {
    if (val) {
      await requestConfigTargets();
    } else {
      form.value?.reset();
    }
  }
);
</script>

<style>
.targets-dialog {
  .targets-dialog__list {
    overflow-y: auto;
  }
}
</style>
