/// <binding BeforeBuild='dev' />
var gulp = require('gulp'), // Gulp JS
    less = require('gulp-less'),
    concat = require('gulp-concat'), // Склейка файлов
    cssmin = require('gulp-cssmin'),
    uglify = require('gulp-uglify'), // Минификация JS
    watch = require('gulp-watch'),
    clean = require('gulp-clean'),
    sourcemaps = require('gulp-sourcemaps');

var paths = {
  js: './Assets/js/**/*.js',
  less: './Assets/css/**/*.less',
  fonts: './Assets/fonts/*.css'
};

// DEV
gulp.task('dev', ['dev:js', 'dev:css']);
gulp.task('dev:js', function () {
  gulp.src(paths.js)
      .pipe(sourcemaps.init())
      .pipe(concat('./content/js/site.js'))
      .pipe(sourcemaps.write('.'))
      .pipe(gulp.dest('.'));
});
gulp.task('dev:css', function () {
  gulp.src(paths.less)
      .pipe(sourcemaps.init())
      .pipe(less())
      .pipe(concat('./content/css/site.css'))
      .pipe(sourcemaps.write('.'))
      .pipe(gulp.dest('.'));
});

// PUB
gulp.task('pub', ['pub:js', 'pub:css']);
gulp.task('pub:js', function () {
  gulp.src(paths.js)
      .pipe(concat('./content/js/site.js'))
      .pipe(uglify())
      .pipe(gulp.dest('.'));
});
gulp.task('pub:css', function () {
  gulp.src(paths.less)
      .pipe(less())
      .pipe(concat('./content/css/site.css'))
      .pipe(cssmin())
      .pipe(gulp.dest('.'));
});


gulp.task('watch', function () {
  gulp.watch(paths.less, ['dev:css']);
  gulp.watch(paths.js, ['dev:js']);
});
