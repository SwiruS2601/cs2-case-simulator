#!/bin/bash
set -e

build_backend() {
  echo "Building backend..."
  docker build --network=host -t 10.10.10.46:5000/cs2-case-unboxer-be ./backend
  docker push 10.10.10.46:5000/cs2-case-unboxer-be:latest
}

build_frontend() {
  echo "Building frontend..."
  docker build -t 10.10.10.46:5000/cs2-case-unboxer-fe ./frontend
  docker push 10.10.10.46:5000/cs2-case-unboxer-fe:latest
}

if [ "$#" -eq 0 ]; then
  build_backend
  build_frontend
else
  for target in "$@"; do
    case "$target" in
      backend)
        build_backend
        ;;
      frontend)
        build_frontend
        ;;
      *)
        echo "Unknown target: $target"
        exit 1
        ;;
    esac
  done
fi