<template>
  <v-card flat>
    <ConfigList></ConfigList>
    <ConfigForm></ConfigForm>
  </v-card>
</template>

<script lang="ts" setup>
import ConfigForm from './config-form.vue';
import ConfigList from './config-list.vue';
</script>
