set -e
mongod --replSet rs0 --bind_ip_all &
MONGOD_PID=$!
until mongosh --quiet --eval "db.adminCommand('ping')" ; do
    echo "Waiting for mongod to start..."
    sleep 1
done

echo "mongod started. Initiating replica set..."
mongosh --quiet --eval "rs.initiate({_id: 'rs0', members: [{ _id: 0, host: 'mongo:27017' }]})"
wait $MONGOD_PID