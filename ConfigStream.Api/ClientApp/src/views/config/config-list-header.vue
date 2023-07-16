<template>
  <v-card flat>
    <v-card-text class="d-flex align-center">
      <v-btn
        @click="onRefresh"
        class="mr-2"
        icon="mdi-refresh"
        variant="flat"
      ></v-btn>
      <v-btn
        @click="onCreate"
        prepend-icon="mdi-plus"
        variant="outlined"
        color="primary"
      >
        Create
      </v-btn>
      <v-spacer></v-spacer>
      <v-responsive max-width="500">
        <v-text-field
          v-model="search"
          label="Search"
          density="compact"
          variant="outlined"
          append-inner-icon="mdi-magnify"
          hide-details
          clearable
          class="py-2"
        >
        </v-text-field>
      </v-responsive>
    </v-card-text>
  </v-card>
</template>
<script setup lang="ts">
import { computed } from 'vue';
import { useConfigStore } from '../../store/config';
const store = useConfigStore();
const search = computed({
  get: () => store.search,
  set: (v?: string) => {
    store.updateSearch(v);
  }
});

const onCreate = store.openCreateConfigDialog;
function onRefresh() {
  return store.requestConfigList();
}
</script>
