<template>
  <v-card flat>
    <v-card-text class="d-flex align-center">
      <v-btn @click="onRefresh" icon="mdi-refresh" variant="flat"></v-btn>
      <v-responsive max-width="500">
        <v-select
          v-model="selectedEnvironments"
          @update:model-value="onSelect"
          :items="environments"
          item-title="name"
          item-value="name"
          density="compact"
          variant="outlined"
          label="Environment"
          class="py-2"
          multiple
          chips
          clearable
          hide-details
        ></v-select>
      </v-responsive>
      <v-spacer></v-spacer>
      <v-responsive max-width="500">
        <v-text-field
          v-model="search"
          label="Search"
          density="compact"
          variant="outlined"
          append-inner-icon="mdi-magnify"
          class="py-2"
          hide-details
          clearable
        >
        </v-text-field>
      </v-responsive>
    </v-card-text>
  </v-card>
</template>
<script setup lang="ts">
import { computed } from 'vue';
import { useConfigValueStore } from '../../store/config-value';
import { useConfigEnvironmentStore } from '@/store/config-environment';
import { storeToRefs } from 'pinia';

const store = useConfigValueStore();
const envStore = useConfigEnvironmentStore();

function onSelect() {
  store.debouncedRequestConfigList();
}

const { items: environments } = storeToRefs(envStore);
const { selectedEnvironments } = storeToRefs(store);

const search = computed({
  get: () => store.search,
  set: (v?: string) => {
    store.updateSearch(v);
  }
});

function onRefresh() {
  store.requestConfigValueList();
  envStore.requestConfigEnvironments();
}
</script>
