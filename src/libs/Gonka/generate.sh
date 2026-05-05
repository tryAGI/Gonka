dotnet tool install --global autosdk.cli --prerelease
rm -rf Generated
autosdk generate openapi.yaml \
  --namespace Gonka \
  --clientClassName GonkaClient \
  --targetFramework net10.0 \
  --output Generated \
  --exclude-deprecated-operations
