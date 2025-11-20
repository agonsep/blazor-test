let clockInterval;
let dotNetHelper;

export function startClock(dotNetReference) {
    dotNetHelper = dotNetReference;
    
    // Update time immediately
    updateTime();
    
    // Update every second
    clockInterval = setInterval(updateTime, 1000);
}

export function stopClock() {
    if (clockInterval) {
        clearInterval(clockInterval);
        clockInterval = null;
    }
}

function updateTime() {
    if (dotNetHelper) {
        const now = new Date();
        
        // Format time for Denver timezone
        const timeString = now.toLocaleTimeString('en-US', {
            timeZone: 'America/Denver',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit',
            hour12: true
        });
        
        dotNetHelper.invokeMethodAsync('UpdateTime', timeString);
    }
}
