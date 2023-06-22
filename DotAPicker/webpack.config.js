const path = require('path')
const prod = process.env.NODE_ENV === 'production';

const HtmlWebpackPlugin = require('html-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

const DIST_REL = 'js/dist'
const DIST_PATH = path.resolve(__dirname, DIST_REL).replace(/\\/g, '/')

module.exports = {
  mode: prod ? 'production' : 'development',
  entry: [ './js/picker/PickerPage.tsx', ],
  output: {
      filename: '[name].js',
      path: __dirname + '/js/dist/',
      devtoolModuleFilenameTemplate (info) {
          return `webpack:///${path.relative(DIST_PATH, info.absoluteResourcePath)}`
      }
  },
  module: {
    rules: [
      {
        test: /\.(ts|tsx)$/,
        exclude: /node_modules/,
        resolve: {
          extensions: ['.ts', '.tsx', '.js', '.json'],
        },
        use: 'ts-loader',
      },
      {
        test: /\.css$/,
        use: [MiniCssExtractPlugin.loader, 'css-loader'],
      },
    ]
  },
  devtool: 'source-map',
  plugins: [
    new HtmlWebpackPlugin({
      template: './js/index.html',
    }),
    new MiniCssExtractPlugin(),
  ],
};