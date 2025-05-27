module.exports = {
  preset: "ts-jest",
  testEnvironment: "jsdom",
  transform: {
    "^.+\\.(ts|tsx)$": "ts-jest",
    "^.+\\.(js|jsx|mjs)$": "babel-jest",
  },
  moduleFileExtensions: ["ts", "tsx", "js", "jsx", "json", "node"],
  coveragePathIgnorePatterns: [
    '/src/App.tsx',
    '/src/index.tsx',
    '/src/reportWebVitals.ts',
    '/src/components/NavigationMenu.tsx',
  ]
};
