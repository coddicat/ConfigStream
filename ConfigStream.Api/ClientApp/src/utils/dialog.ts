import { ConfirmDialog, PromptDialog, useDialogStore } from '@/store/dialog';

const store = useDialogStore();

export async function confirmDialog(message: string, title?: string) {
  return new Promise<boolean>(resolve => {
    const data: ConfirmDialog = {
      dialog: true,
      message,
      title,
      resolve
    };
    store.setConfirmDialog(data);
  });
}

export async function promptDialog(
  message: string,
  title?: string,
  rules?: ((value?: string) => string | boolean)[]
) {
  return new Promise<string | undefined>(resolve => {
    const data: PromptDialog = {
      dialog: true,
      message,
      title,
      rules,
      resolve
    };

    store.setPromptDialog(data);
  });
}
