import { serverConfig } from '~/config.server';

export default defineEventHandler(async (event) => {
  const name = getRouterParam(event, 'name');
  const crates = await $fetch(`${serverConfig.apiUrl}/api/crates/name/${name}`);
  return crates;
});
