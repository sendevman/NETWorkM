"use strict";(self.webpackChunknetworkmanager=self.webpackChunknetworkmanager||[]).push([[8911],{9894:(e,n,s)=>{s.r(n),s.d(n,{assets:()=>c,contentTitle:()=>t,default:()=>a,frontMatter:()=>i,metadata:()=>o,toc:()=>l});var r=s(4848),d=s(8453);const i={sidebar_position:4},t="Port Scanner",o={id:"application/port-scanner",title:"Port Scanner",description:"With the Port Scanner you can scan for open tcp ports on one or multiple hosts to determine which services are running.",source:"@site/docs/application/port-scanner.md",sourceDirName:"application",slug:"/application/port-scanner",permalink:"/NETworkManager/docs/application/port-scanner",draft:!1,unlisted:!1,editUrl:"https://github.com/bornToBeRoot/NETworkManager/tree/main/Website/docs/application/port-scanner.md",tags:[],version:"current",sidebarPosition:4,frontMatter:{sidebar_position:4},sidebar:"docsSidebar",previous:{title:"IP Scanner",permalink:"/NETworkManager/docs/application/ip-scanner"},next:{title:"Ping Monitor",permalink:"/NETworkManager/docs/application/ping-monitor"}},c={},l=[{value:"Profile",id:"profile",level:2},{value:"Inherit host from general",id:"inherit-host-from-general",level:3},{value:"Host",id:"host",level:3},{value:"Ports",id:"ports",level:3},{value:"Settings",id:"settings",level:2},{value:"Port profiles",id:"port-profiles",level:3},{value:"Show closed ports",id:"show-closed-ports",level:3},{value:"Timeout (ms)",id:"timeout-ms",level:3},{value:"Resolve hostname",id:"resolve-hostname",level:3},{value:"Max. concurrent host threads",id:"max-concurrent-host-threads",level:3},{value:"Max. concurrent port threads",id:"max-concurrent-port-threads",level:3}];function h(e){const n={a:"a",admonition:"admonition",code:"code",h1:"h1",h2:"h2",h3:"h3",header:"header",img:"img",li:"li",p:"p",strong:"strong",table:"table",tbody:"tbody",td:"td",th:"th",thead:"thead",tr:"tr",ul:"ul",...(0,d.R)(),...e.components};return(0,r.jsxs)(r.Fragment,{children:[(0,r.jsx)(n.header,{children:(0,r.jsx)(n.h1,{id:"port-scanner",children:"Port Scanner"})}),"\n",(0,r.jsxs)(n.p,{children:["With the ",(0,r.jsx)(n.strong,{children:"Port Scanner"})," you can scan for open ",(0,r.jsx)(n.code,{children:"tcp"})," ports on one or multiple hosts to determine which services are running."]}),"\n",(0,r.jsx)(n.p,{children:"Example inputs:"}),"\n",(0,r.jsxs)(n.table,{children:[(0,r.jsx)(n.thead,{children:(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.th,{children:"Host"}),(0,r.jsx)(n.th,{children:"Description"})]})}),(0,r.jsxs)(n.tbody,{children:[(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"10.0.0.1"})}),(0,r.jsxs)(n.td,{children:["Single IP address (",(0,r.jsx)(n.code,{children:"10.0.0.1"}),")"]})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"10.0.0.100 - 10.0.0.199"})}),(0,r.jsxs)(n.td,{children:["All IP addresses in a given range (",(0,r.jsx)(n.code,{children:"10.0.0.100"}),", ",(0,r.jsx)(n.code,{children:"10.0.0.101"}),", ..., ",(0,r.jsx)(n.code,{children:"10.0.0.199"}),")"]})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"10.0.0.0/23"})}),(0,r.jsxs)(n.td,{children:["All IP addresses in a subnet (",(0,r.jsx)(n.code,{children:"10.0.0.0"}),", ..., ",(0,r.jsx)(n.code,{children:"10.0.1.255"}),")"]})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"10.0.0.0/255.255.254.0"})}),(0,r.jsxs)(n.td,{children:["All IP addresses in a subnet (",(0,r.jsx)(n.code,{children:"10.0.0.0"}),", ..., ",(0,r.jsx)(n.code,{children:"10.0.1.255"}),")"]})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"10.0.[0-9,20].[1-2]"})}),(0,r.jsxs)(n.td,{children:["Multipe IP address like (",(0,r.jsx)(n.code,{children:"10.0.0.1"}),", ",(0,r.jsx)(n.code,{children:"10.0.0.2"}),", ",(0,r.jsx)(n.code,{children:"10.0.1.1"}),", ...,",(0,r.jsx)(n.code,{children:"10.0.9.2"}),", ",(0,r.jsx)(n.code,{children:"10.0.20.1"}),")"]})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"borntoberoot.net"})}),(0,r.jsxs)(n.td,{children:["Single IP address resolved from a host (",(0,r.jsx)(n.code,{children:"10.0.0.1"}),")"]})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"borntoberoot.net/24"})}),(0,r.jsxs)(n.td,{children:["All IP addresses in a subnet resolved from a host (",(0,r.jsx)(n.code,{children:"10.0.0.0"}),", ..., ",(0,r.jsx)(n.code,{children:"10.0.0.255"}),")"]})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"borntoberoot.net/255.255.255.0"})}),(0,r.jsxs)(n.td,{children:["All IP addresses in a subnet resolved from a host (",(0,r.jsx)(n.code,{children:"10.0.0.0"}),", ..., ",(0,r.jsx)(n.code,{children:"10.0.0.255"}),")"]})]})]})]}),"\n",(0,r.jsxs)(n.table,{children:[(0,r.jsx)(n.thead,{children:(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.th,{children:"Port"}),(0,r.jsx)(n.th,{children:"Description"})]})}),(0,r.jsxs)(n.tbody,{children:[(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"1-1024"})}),(0,r.jsxs)(n.td,{children:["All ports in a given range (",(0,r.jsx)(n.code,{children:"1"}),", ",(0,r.jsx)(n.code,{children:"2"}),", ..., ",(0,r.jsx)(n.code,{children:"1024"}),")"]})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"80; 443; 8080; 8443"})}),(0,r.jsxs)(n.td,{children:["Multiple ports like (",(0,r.jsx)(n.code,{children:"80"}),", ",(0,r.jsx)(n.code,{children:"443"}),", ",(0,r.jsx)(n.code,{children:"8080"}),", ",(0,r.jsx)(n.code,{children:"8443"}),")"]})]})]})]}),"\n",(0,r.jsxs)(n.admonition,{type:"note",children:[(0,r.jsxs)(n.p,{children:["Multiple inputs can be combined with a semicolon (",(0,r.jsx)(n.code,{children:";"}),")."]}),(0,r.jsxs)(n.p,{children:["Example: ",(0,r.jsx)(n.code,{children:"10.0.0.0/24; 10.0.[10-20]1"})," or ",(0,r.jsx)(n.code,{children:"1-1024; 8080; 8443"})]})]}),"\n",(0,r.jsx)(n.p,{children:(0,r.jsx)(n.img,{alt:"Port Scanner",src:s(1126).A+"",width:"1200",height:"800"})}),"\n",(0,r.jsx)(n.admonition,{type:"note",children:(0,r.jsx)(n.p,{children:"Right-click on the result to copy or export the information."})}),"\n",(0,r.jsx)(n.h2,{id:"profile",children:"Profile"}),"\n",(0,r.jsx)(n.h3,{id:"inherit-host-from-general",children:"Inherit host from general"}),"\n",(0,r.jsx)(n.p,{children:"Inherit the host from the general settings."}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Type:"})," ",(0,r.jsx)(n.code,{children:"Boolean"})]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Default:"})," ",(0,r.jsx)(n.code,{children:"Enabled"})]}),"\n",(0,r.jsx)(n.admonition,{type:"note",children:(0,r.jsxs)(n.p,{children:["If this option is enabled, the ",(0,r.jsx)(n.a,{href:"#host",children:"Host"})," is overwritten by the host from the general settings and the ",(0,r.jsx)(n.a,{href:"#host",children:"Host"})," is disabled."]})}),"\n",(0,r.jsx)(n.h3,{id:"host",children:"Host"}),"\n",(0,r.jsx)(n.p,{children:"Hostname or IP range to scan for open ports."}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Type:"})," ",(0,r.jsx)(n.code,{children:"String"})]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Default:"})," ",(0,r.jsx)(n.code,{children:"Empty"})]}),"\n",(0,r.jsx)(n.p,{children:(0,r.jsx)(n.strong,{children:"Example:"})}),"\n",(0,r.jsxs)(n.ul,{children:["\n",(0,r.jsx)(n.li,{children:(0,r.jsx)(n.code,{children:"server-01.borntoberoot.net"})}),"\n",(0,r.jsx)(n.li,{children:(0,r.jsx)(n.code,{children:"1.1.1.1; 1.0.0.1"})}),"\n",(0,r.jsx)(n.li,{children:(0,r.jsx)(n.code,{children:"10.0.0.0/24"})}),"\n"]}),"\n",(0,r.jsx)(n.admonition,{type:"note",children:(0,r.jsxs)(n.p,{children:["See also the ",(0,r.jsx)(n.a,{href:"./port-scanner",children:"Port Scanner"})," example inputs for more information about the supported host formats."]})}),"\n",(0,r.jsx)(n.h3,{id:"ports",children:"Ports"}),"\n",(0,r.jsx)(n.p,{children:"TCP ports to scan each host for."}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Type:"})," ",(0,r.jsx)(n.code,{children:"String"})]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Default:"})," ",(0,r.jsx)(n.code,{children:"Empty"})]}),"\n",(0,r.jsx)(n.p,{children:(0,r.jsx)(n.strong,{children:"Example:"})}),"\n",(0,r.jsxs)(n.ul,{children:["\n",(0,r.jsx)(n.li,{children:(0,r.jsx)(n.code,{children:"1-1024"})}),"\n",(0,r.jsx)(n.li,{children:(0,r.jsx)(n.code,{children:"80; 443; 8080; 8443"})}),"\n"]}),"\n",(0,r.jsx)(n.admonition,{type:"note",children:(0,r.jsxs)(n.p,{children:["See also the ",(0,r.jsx)(n.a,{href:"./port-scanner",children:"Port Scanner"})," example inputs for more information about the supported port formats."]})}),"\n",(0,r.jsx)(n.h2,{id:"settings",children:"Settings"}),"\n",(0,r.jsx)(n.h3,{id:"port-profiles",children:"Port profiles"}),"\n",(0,r.jsxs)(n.p,{children:["List of common ",(0,r.jsx)(n.code,{children:"tcp"})," ports to scan for."]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Type:"})," ",(0,r.jsx)(n.code,{children:"List<NETworkManager.Models.Network.PortProfileInfo>"})]}),"\n",(0,r.jsx)(n.p,{children:(0,r.jsx)(n.strong,{children:"Default:"})}),"\n",(0,r.jsxs)(n.table,{children:[(0,r.jsx)(n.thead,{children:(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.th,{children:"Name"}),(0,r.jsx)(n.th,{children:"Ports"})]})}),(0,r.jsxs)(n.tbody,{children:[(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"DNS (via TCP)"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"53"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"NTP (via TCP)"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"123"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"Webserver"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"80; 443"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"Webserver (Other)"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"80; 443; 8080; 8443"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"Remote access"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"22; 23; 3389; 5900"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"Mailserver"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"25; 110; 143; 465; 587; 993; 995"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"Filetransfer"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"20-21; 22; 989-990; 2049"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"Database"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"1433-1434; 1521; 1830; 3306; 5432"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"SMB"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"139; 445"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"LDAP"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"389; 636"})})]}),(0,r.jsxs)(n.tr,{children:[(0,r.jsx)(n.td,{children:"HTTP proxy"}),(0,r.jsx)(n.td,{children:(0,r.jsx)(n.code,{children:"3128"})})]})]})]}),"\n",(0,r.jsx)(n.h3,{id:"show-closed-ports",children:"Show closed ports"}),"\n",(0,r.jsx)(n.p,{children:"Show closed ports in the result list."}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Type:"})," ",(0,r.jsx)(n.code,{children:"Boolean"})]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Default:"})," ",(0,r.jsx)(n.code,{children:"Disabled"})]}),"\n",(0,r.jsx)(n.h3,{id:"timeout-ms",children:"Timeout (ms)"}),"\n",(0,r.jsx)(n.p,{children:"Timeout in milliseconds after which a port is considered closed / timed out."}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Type:"})," ",(0,r.jsx)(n.code,{children:"Integer"})," [Min ",(0,r.jsx)(n.code,{children:"100"}),", Max ",(0,r.jsx)(n.code,{children:"15000"}),"]"]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Default:"})," ",(0,r.jsx)(n.code,{children:"4000"})]}),"\n",(0,r.jsx)(n.h3,{id:"resolve-hostname",children:"Resolve hostname"}),"\n",(0,r.jsx)(n.p,{children:"Resolve the hostname for given IP addresses."}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Type:"})," ",(0,r.jsx)(n.code,{children:"Boolean"})]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Default:"})," ",(0,r.jsx)(n.code,{children:"Enabled"})]}),"\n",(0,r.jsx)(n.h3,{id:"max-concurrent-host-threads",children:"Max. concurrent host threads"}),"\n",(0,r.jsx)(n.p,{children:"Maximum number of threads used to scan hosts (1 thread = 1 host)."}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Type:"})," ",(0,r.jsx)(n.code,{children:"Integer"})," [Min ",(0,r.jsx)(n.code,{children:"1"}),", Max ",(0,r.jsx)(n.code,{children:"10"}),"]"]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Default:"})," ",(0,r.jsx)(n.code,{children:"5"})]}),"\n",(0,r.jsxs)(n.admonition,{type:"warning",children:[(0,r.jsx)(n.p,{children:"Too many simultaneous requests may be blocked by a firewall. You can reduce the number of threads to avoid this, but this will increase the scan time."}),(0,r.jsx)(n.p,{children:"Too many threads can also cause performance problems on the device."})]}),"\n",(0,r.jsx)(n.admonition,{type:"note",children:(0,r.jsxs)(n.p,{children:["This setting only change the maximum number of concurrently executed threads per host scan. See also the ",(0,r.jsx)(n.a,{href:"../settings/general#threadpool-additional-min-threads",children:"General"})," settings to configure the application wide thread pool."]})}),"\n",(0,r.jsx)(n.h3,{id:"max-concurrent-port-threads",children:"Max. concurrent port threads"}),"\n",(0,r.jsx)(n.p,{children:"Maximum number of threads used to scan for ports for each host (1 thread = 1 port per host)."}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Type:"})," ",(0,r.jsx)(n.code,{children:"Integer"})," [Min ",(0,r.jsx)(n.code,{children:"1"}),", Max ",(0,r.jsx)(n.code,{children:"512"}),"]"]}),"\n",(0,r.jsxs)(n.p,{children:[(0,r.jsx)(n.strong,{children:"Default:"})," ",(0,r.jsx)(n.code,{children:"256"})]}),"\n",(0,r.jsxs)(n.admonition,{type:"warning",children:[(0,r.jsx)(n.p,{children:"Too many simultaneous requests may be blocked by a firewall. You can reduce the number of threads to avoid this, but this will increase the scan time."}),(0,r.jsx)(n.p,{children:"Too many threads can also cause performance problems on the device."})]}),"\n",(0,r.jsx)(n.admonition,{type:"note",children:(0,r.jsxs)(n.p,{children:["This setting only change the maximum number of concurrently executed threads per host scan. See also the ",(0,r.jsx)(n.a,{href:"../settings/general#threadpool-additional-min-threads",children:"General"})," settings to configure the application wide thread pool."]})})]})}function a(e={}){const{wrapper:n}={...(0,d.R)(),...e.components};return n?(0,r.jsx)(n,{...e,children:(0,r.jsx)(h,{...e})}):h(e)}},1126:(e,n,s)=>{s.d(n,{A:()=>r});const r=s.p+"assets/images/port-scanner-1e2e1773621207ef4872dd2898a12b90.png"},8453:(e,n,s)=>{s.d(n,{R:()=>t,x:()=>o});var r=s(6540);const d={},i=r.createContext(d);function t(e){const n=r.useContext(i);return r.useMemo((function(){return"function"==typeof e?e(n):{...n,...e}}),[n,e])}function o(e){let n;return n=e.disableParentContext?"function"==typeof e.components?e.components(d):e.components||d:t(e.components),r.createElement(i.Provider,{value:n},e.children)}}}]);