export const BOOK_TABS = [
  { id: 'summary', label: 'Summary' },
  { id: 'basic-info', label: 'Basic Info' },
  { id: 'reviews', label: 'Reviews' },
] as const

export type BookTabId = (typeof BOOK_TABS)[number]['id']

export const getTabId = (id: BookTabId) => `book-tab-${id}`
export const getPanelId = (id: BookTabId) => `book-panel-${id}`

export const DEFAULT_DESCRIPTION =
  'Every book is a door waiting to be opened. This one has not shared its summary yet, but that is part of the magic: some stories are best discovered without a map. ' +
  'Somewhere between its first and last page there may be a place you have never been, a character who feels like an old friend, or a sentence you will want to read twice. ' +
  'Maybe it will make you laugh on a quiet afternoon, keep you awake past midnight, or change the way you see something you thought you already understood. ' +
  'Books have always been the most patient companions: they wait on the shelf until the right reader arrives, and they never rush anyone through a chapter. ' +
  'They do not mind if you read them slowly, if you pause to stare out of the window, or if you come back to them months later and find them quietly changed, because it was you who changed. ' +
  'That is the beauty of reading: every visit to the same pages can feel like meeting the story for the very first time. ' +
  'So take your time. Look at the cover, check the author and the details in Basic Info, and let your curiosity decide. ' +
  'If something about it calls to you, borrow it, find a comfortable spot, make yourself a warm drink, and turn the first page. ' +
  'The best part of any story is the moment you stop wondering what it is about and start living it.'