// Jest ejecuta CommonJS y no entiende `import.meta` (lo usa react-router: import.meta.hot).
// Este plugin lo reemplaza por un objeto vacío, solo durante las pruebas.
module.exports = ({ types: t }) => ({
  visitor: {
    MetaProperty(path) {
      if (path.node.meta.name === 'import' && path.node.property.name === 'meta') {
        path.replaceWith(t.objectExpression([]))
      }
    },
  },
})
