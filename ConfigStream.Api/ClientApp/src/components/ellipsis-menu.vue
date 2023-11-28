<script setup lang="ts">
import { ref } from 'vue';
import { MenuItem } from 'primevue/menuitem';
defineProps<{
  items: MenuItem[];
  disabled: boolean;
}>();
const menu = ref();
function toggle(event: MouseEvent) {
  menu.value.toggle(event);
}
</script>
<template>
  <Button
    @click="toggle"
    :disabled="disabled"
    icon="pi pi-ellipsis-v"
    size="small"
    class="flex-shrink-0"
    text
    rounded
  ></Button>
  <TieredMenu ref="menu" :model="items" popup></TieredMenu>
</template>
