// export function useWebSocketClient() {
//     const socket = ref<WebSocket | null>(null);
//     const isConnected = ref(false);
//     const messages = ref<string[]>([]);
//     let timeout: NodeJS.Timeout | undefined = undefined;
//     // const unboxFeed = ref<any[]>([]);

//     // SignalR protocol constants
//     const endChar = String.fromCharCode(30);

//     function connect() {
//         // First, negotiate the connection
//         fetch('http://localhost:5015/unboxHub/negotiate?negotiateVersion=1')
//             .then((response) => response.json())
//             .then((negotiateResponse) => {
//                 const connectionId = negotiateResponse.connectionId;
//                 const wsUrl = `ws://localhost:5015/unboxHub?id=${connectionId}`;

//                 socket.value = new WebSocket(wsUrl);

//                 socket.value.onopen = () => {
//                     isConnected.value = true;
//                     console.log('WebSocket connected');

//                     // Send the handshake message for SignalR
//                     if (socket.value) {
//                         socket.value.send(JSON.stringify({ protocol: 'json', version: 1 }) + endChar);
//                     }
//                 };

//                 socket.value.onmessage = (event) => {
//                     const data = event.data;
//                     if (typeof data === 'string') {
//                         // Split by SignalR protocol delimiter
//                         const messages = data.split(endChar);

//                         for (const msg of messages) {
//                             if (!msg) continue;

//                             try {
//                                 const parsedMsg = JSON.parse(msg);

//                                 // Handle different message types
//                                 if (parsedMsg.type === 1) {
//                                     // Invocation message
//                                     if (parsedMsg.target === 'receiveRareUnbox') {
//                                         handleRareUnbox(parsedMsg.arguments);
//                                     }
//                                 }
//                             } catch (e) {
//                                 console.error('Error parsing message:', e);
//                             }
//                         }
//                     }
//                 };

//                 socket.value.onclose = () => {
//                     isConnected.value = false;
//                     console.log('WebSocket disconnected');
//                 };

//                 socket.value.onerror = (error) => {
//                     console.error('WebSocket error:', error);
//                 };
//             })
//             .catch((error) => {
//                 console.error('Negotiation failed:', error);

//                 // Retry connection
//                 timeout = setTimeout(() => {
//                     connect();
//                 }, 5000);
//             });
//     }

//     // function handleRareUnbox([country, itemName, rarity, imageUrl]: [string, string, string, string]) {
//     //     unboxFeed.value.unshift({
//     //         id: Date.now(),
//     //         country,
//     //         itemName,
//     //         rarity,
//     //         imageUrl,
//     //         timestamp: new Date().toISOString()
//     //     });

//     //     // Keep feed limited to latest items
//     //     if (unboxFeed.value.length > 50) {
//     //         unboxFeed.value = unboxFeed.value.slice(0, 50);
//     //     }
//     // }

//     function sendMessage(message: string) {
//         if (socket.value && isConnected.value) {
//             socket.value.send(message);
//         } else {
//             console.error('WebSocket is not connected');
//         }
//     }

//     function disconnect() {
//         if (socket.value) {
//             socket.value.close();
//         }
//     }

//     onMounted(() => {
//         connect();

//         setInterval(() => {
//             if (socket.value && isConnected.value) {
//                 socket.value.send(`{"arguments":[],"invocationId":"0","target":"Your-Method","type":1}${endChar}`);
//             }
//         }, 1000);
//     });

//     watchEffect(() => {
//         if (!isConnected) {
//             timeout = setTimeout(() => {
//                 connect();
//             }, 5000);
//         }
//     });

//     onBeforeUnmount(() => {
//         disconnect();
//         if (timeout) {
//             clearTimeout(timeout);
//         }
//     });

//     return {
//         connect,
//         sendMessage,
//         disconnect,
//         isConnected,
//         messages,
//         // unboxFeed
//     };
// }
