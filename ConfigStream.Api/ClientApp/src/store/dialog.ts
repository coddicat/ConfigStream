import { defineStore } from 'pinia';

export type ConfirmDialog = {
  resolve: (value: boolean | PromiseLike<boolean>) => void;
  title?: string;
  message: string;
  dialog: boolean;
};

export type PromptDialog = {
  resolve: (
    value: string | undefined | PromiseLike<string | undefined>
  ) => void;
  title?: string;
  message: string;
  dialog: boolean;
  validate?: (value?: string) => string | boolean;
};

type Store = {
  confirmDialog?: ConfirmDialog;
  promptDialog?: PromptDialog;
};

export const useDialogStore = defineStore('dialog', {
  state: (): Store => ({
    confirmDialog: undefined,
    promptDialog: undefined
  }),
  actions: {
    setConfirmDialog(confirmDialog: ConfirmDialog) {
      this.confirmDialog = confirmDialog;
    },
    setPromptDialog(promptDialog: PromptDialog) {
      this.promptDialog = promptDialog;
    }
  }
});
