<!-- eslint-disable vue/valid-v-slot -->
<template>
  <v-main>
    <v-app-bar elevation="1" color="primary">
      <v-app-bar-nav-icon @click="switchRail"></v-app-bar-nav-icon>
      <v-toolbar-title>ConfigStream</v-toolbar-title>
      <v-spacer></v-spacer>
    </v-app-bar>
    <v-navigation-drawer :rail="data.rail" expand-on-hover permanent>
      <v-list>
        <v-list-item
          prepend-icon="mdi-format-list-bulleted"
          :to="{ name: 'Values' }"
        >
          Values
        </v-list-item>
        <v-list-item prepend-icon="mdi-cogs" :to="{ name: 'Configs' }">
          Configs
        </v-list-item>
        <v-list-item prepend-icon="mdi-target">Targets</v-list-item>
      </v-list>

      <v-divider></v-divider>

      <v-list>
        <v-list-item prepend-icon="mdi-group" @click="openEnvironments">
          Environments
          <template v-slot:append>
            <v-icon size="small">mdi-open-in-new</v-icon>
          </template>
        </v-list-item>

        <v-list-item prepend-icon="mdi-group" @click="openGroups">
          Groups
          <template v-slot:append>
            <v-icon size="small">mdi-open-in-new</v-icon>
          </template>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
    <RouterView></RouterView>
    <groups-dialog v-model="data.groupListDialog"></groups-dialog>
    <environments-dialog
      v-model="data.environmentListDialog"
    ></environments-dialog>
  </v-main>
</template>

<script lang="ts" setup>
import groupsDialog from '@/views/group/groups-dialog.vue';
import environmentsDialog from '@/views/environment/environments-dialog.vue';
import { reactive } from 'vue';

const data = reactive({
  rail: false,
  groupListDialog: false,
  environmentListDialog: false
});

function switchRail() {
  data.rail = !data.rail;
}
function openGroups() {
  data.groupListDialog = true;
}
function openEnvironments() {
  data.environmentListDialog = true;
}
</script>
