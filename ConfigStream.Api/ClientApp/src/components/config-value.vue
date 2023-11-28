<script setup lang="ts">
import { computed } from 'vue';
import { ConfigValue } from '@/store/config-value';
import { Config } from '@/store/config';
import ConfigValueChip from './config-value-chip.vue';
const props = defineProps<{
  configValue: ConfigValue;
  config: Config;
  edit?: boolean;
  disabled?: boolean;
}>();

const allowedValues = computed(() => props.config.allowedValues);
const _configValue = computed(() => props.configValue);
const model = computed({
  get: () => _configValue.value.value,
  set: (v: string) => (_configValue.value.value = v)
});
</script>

<template>
  <template v-if="edit">
    <div class="flex flex-row gap-2 align-items-center">
      <Dropdown
        v-if="allowedValues?.length > 0"
        v-model="model"
        :options="allowedValues"
        :disabled="disabled"
        class="w-full"
      >
      </Dropdown>
      <InputText
        v-else
        v-model="model"
        :disabled="disabled"
        @keydown.stop="() => {}"
        class="w-full"
      />
      <Button
        icon="pi pi-eraser"
        text
        rounded
        severity="danger"
        class="flex-grow-0 flex-shrink-0 force-small-button"
        @click="() => (model = undefined)"
      />
    </div>
  </template>
  <ConfigValueChip v-else :label="model" />
</template>
