#!/bin/bash
set -e

# Function to build the backend image
# Usage: build_backend [tag]
build_backend() {
  local tag="${1:-latest}" # Default to 'latest' if $1 is empty or unset
  local image_name="10.10.10.46:5000/cs2-case-unboxer-be"
  local full_image_name="${image_name}:${tag}"

  echo "Building backend image: ${full_image_name}..."
  docker build --network=host -t "${full_image_name}" ./Cs2CaseOpener
  
  echo "Pushing image ${full_image_name} to registry..."
  docker push "${full_image_name}"
}

# Function to build the frontend image
# Usage: build_frontend [tag]
build_frontend() {
  local tag="${1:-latest}" # Default to 'latest' if $1 is empty or unset
  local image_name="10.10.10.46:5000/cs2-case-unboxer-fe"
  local full_image_name="${image_name}:${tag}"

  echo "Building frontend image: ${full_image_name}..."
  docker build -t "${full_image_name}" ./frontend
  
  echo "Pushing image ${full_image_name} to registry..."
  docker push "${full_image_name}"
}

# --- Main Script Logic ---

if [ "$#" -eq 0 ]; then
  # No arguments provided, build both with default 'latest' tag
  build_backend
  build_frontend
elif [ "$#" -eq 1 ]; then
  # One argument: target (backend or frontend), use default 'latest' tag
  target="$1"
  case "$target" in
    backend)
      build_backend
      ;;
    frontend)
      build_frontend
      ;;
    *)
      echo "Unknown target: $target" >&2 # Error message to stderr
      exit 1
      ;;
  esac
elif [ "$#" -eq 2 ]; then
  # Two arguments: target and tag
  target="$1"
  tag="$2"
  case "$target" in
    backend)
      build_backend "$tag"
      ;;
    frontend)
      build_frontend "$tag"
      ;;
    *)
      echo "Unknown target: $target" >&2 # Error message to stderr
      exit 1
      ;;
  esac
else
  # More than two arguments
  echo "Usage: $0 [backend|frontend] [tag]" >&2 # Error message to stderr
  exit 1
fi