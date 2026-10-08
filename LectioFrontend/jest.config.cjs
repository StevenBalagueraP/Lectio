/** @type {import('jest').Config} */
module.exports = {
  testEnvironment: 'jsdom',
  roots: ['<rootDir>/src'],
  testMatch: ['**/*.test.ts', '**/*.test.tsx'],
  setupFiles: ['<rootDir>/src/test/polyfills.ts'],
  setupFilesAfterEnv: ['<rootDir>/src/test/setupTests.ts'],

  // Babel compila TS/TSX y también react-router y @remix-run/* (se publican solo como ESM).
  transform: {
    '^.+\\.[jt]sx?$': [
      'babel-jest',
      {
        plugins: ['./jest.babel-import-meta.cjs'],
        presets: [
          ['@babel/preset-env', { targets: { node: 'current' } }],
          ['@babel/preset-react', { runtime: 'automatic' }],
          '@babel/preset-typescript',
        ],
      },
    ],
  },
  transformIgnorePatterns: ['/node_modules/(?!(react-router|@remix-run)/)'],

  moduleNameMapper: {
    '\\.module\\.css$': 'identity-obj-proxy',
    '\\.css$': '<rootDir>/src/test/styleMock.ts',
    // react-router (módulo ESM-only) importa cookie-es, que solo existe como .mjs.
    '^cookie-es$': '<rootDir>/src/test/cookieEsMock.ts',
    // config/env.ts usa import.meta.env (Vite), que Jest no entiende.
    '^(\\.{1,2}/)+config/env$': '<rootDir>/src/test/envMock.ts',
  },

  clearMocks: true,
}