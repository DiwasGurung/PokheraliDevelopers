import React, { useState } from "react";
import { Package, CheckCircle, XCircle, AlertCircle, Clipboard, Check } from "lucide-react";

export default function ClaimCodeProcessor() {
  const [claimCode, setClaimCode] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const [orderDetails, setOrderDetails] = useState(null);
  const [copied, setCopied] = useState(false);

  const handleClaimCodeChange = (e) => {
    // Convert to uppercase and remove spaces
    const value = e.target.value.toUpperCase().replace(/\s/g, '');
    setClaimCode(value);
  };

  const processClaimCode = async (e) => {
    e.preventDefault();
    
    if (!claimCode) return;
    
    try {
      setLoading(true);
      setError(null);
      setSuccess(false);
      setOrderDetails(null);
      
      const token = localStorage.getItem('token');
      
      if (!token) {
        throw new Error('Not authenticated');
      }
      
      const response = await fetch('https://localhost:7126/api/Orders/claim-code', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(claimCode)
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Invalid or used claim code');
      }
      
      const data = await response.json();
      setSuccess(true);
      setOrderDetails(data.order);
      
      // Broadcast the successful order processing
      broadcastOrderFulfillment(data.order);
    } catch (err) {
      console.error('Error processing claim code:', err);
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };
  
  const broadcastOrderFulfillment = (order) => {
    // In a real application, this would use a WebSocket or SignalR connection
    console.log('Broadcasting order fulfillment:', order);
    // Example notification that would be sent to all connected clients:
    // "Order #ORD-12345 has been fulfilled: 'The Great Adventure' by John Doe and 2 other books"
  };
  
  const handleCopyClaimCode = () => {
    navigator.clipboard.writeText(claimCode);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };
  
  const resetForm = () => {
    setClaimCode("");
    setSuccess(false);
    setOrderDetails(null);
    setError(null);
  };
  
  // Format date for display
  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div className="max-w-3xl mx-auto">
      <div className="bg-white rounded-xl shadow-md overflow-hidden">
        <div className="p-6">
          <div className="flex items-center justify-center mb-6">
            <Package size={36} className="text-purple-600 mr-3" />
            <h1 className="text-2xl font-bold text-gray-900">Order Claim Code Processor</h1>
          </div>
          
          <p className="text-gray-600 text-center mb-8">
            Enter the claim code provided by the customer to process their order.
          </p>
          
          {error && (
            <div className="bg-red-50 text-red-600 p-4 rounded-lg mb-6 flex items-start">
              <XCircle size={20} className="mr-2 flex-shrink-0 mt-0.5" />
              <p>{error}</p>
            </div>
          )}
          
          {success && orderDetails && (
            <div className="bg-green-50 text-green-600 p-4 rounded-lg mb-6 flex items-start">
              <CheckCircle size={20} className="mr-2 flex-shrink-0 mt-0.5" />
              <p>Order successfully processed!</p>
            </div>
          )}
          
          <form onSubmit={processClaimCode} className="mb-6">
            <div className="flex flex-col sm:flex-row gap-3">
              <div className="relative flex-grow">
                <input
                  type="text"
                  value={claimCode}
                  onChange={handleClaimCodeChange}
                  placeholder="Enter 6-character claim code"
                  maxLength={6}
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-purple-500 text-lg font-mono tracking-widest uppercase text-center"
                  disabled={loading || success}
                />
                {claimCode && (
                  <button
                    type="button"
                    onClick={handleCopyClaimCode}
                    className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600 focus:outline-none"
                    title="Copy claim code"
                  >
                    {copied ? <Check size={20} className="text-green-500" /> : <Clipboard size={20} />}
                  </button>
                )}
              </div>
              <button
                type="submit"
                disabled={!claimCode || loading || success}
                className={`px-6 py-3 rounded-lg font-medium ${
                  !claimCode || loading || success
                    ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                    : 'bg-purple-600 text-white hover:bg-purple-700'
                } transition-colors`}
              >
                {loading ? 'Processing...' : 'Process Code'}
              </button>
            </div>
          </form>
          
          {success && orderDetails && (
            <div className="border border-gray-200 rounded-lg overflow-hidden">
              <div className="bg-gray-50 px-4 py-3 border-b border-gray-200">
                <h2 className="font-semibold text-gray-800">Order Details</h2>
              </div>
              
              <div className="p-4 space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <p className="text-sm text-gray-500">Order Number</p>
                    <p className="font-medium">{orderDetails.orderNumber}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Date</p>
                    <p className="font-medium">{formatDate(orderDetails.createdAt)}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Total Amount</p>
                    <p className="font-medium">${orderDetails.totalAmount.toFixed(2)}</p>
                  </div>
                  <div>
                    <p className="text-sm text-gray-500">Status</p>
                    <p className="font-medium">{orderDetails.orderStatus}</p>
                  </div>
                </div>
                
                <div className="mt-4">
                  <h3 className="font-medium text-gray-800 mb-2">Items</h3>
                  <div className="space-y-3">
                    {orderDetails.items.map((item) => (
                      <div key={item.id} className="flex items-start gap-3 p-3 bg-gray-50 rounded-lg">
                        <div className="h-14 w-10 bg-gray-200 rounded overflow-hidden flex-shrink-0">
                          {item.bookImageUrl && (
                            <img 
                              src={item.bookImageUrl} 
                              alt={item.bookTitle} 
                              className="h-full w-full object-cover"
                            />
                          )}
                        </div>
                        <div className="flex-grow">
                          <p className="font-medium">{item.bookTitle}</p>
                          <div className="flex justify-between mt-1">
                            <p className="text-sm text-gray-500">Qty: {item.quantity}</p>
                            <p className="text-sm font-medium">${item.totalPrice.toFixed(2)}</p>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
                
                <div className="flex justify-end pt-4">
                  <button
                    onClick={resetForm}
                    className="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors"
                  >
                    Process Another Order
                  </button>
                </div>
              </div>
            </div>
          )}
          
          <div className="mt-8 p-4 bg-blue-50 rounded-lg flex items-start">
            <AlertCircle size={20} className="text-blue-500 mr-2 flex-shrink-0 mt-0.5" />
            <div>
              <p className="text-blue-700 font-medium mb-1">Staff Instructions</p>
              <ol className="text-blue-600 text-sm list-decimal pl-4 space-y-1">
                <li>Ask the customer for their claim code (6 characters).</li>
                <li>Enter the code in the form above and click "Process Code".</li>
                <li>Verify the order details with the customer.</li>
                <li>Collect the ordered items and hand them to the customer.</li>
                <li>Mark the order as "Delivered" in the admin dashboard after handing over the items.</li>
              </ol>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}